// File: Caching/RedisCacheService.cs
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using LandingPageApp.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using Polly;
using Polly.Retry;

namespace LandingPageApp.Infrastructure.Caching;

public class RedisCacheService : ICacheRediservice
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer? _redis;
    private readonly ILogger<RedisCacheService>? _logger;
    
    // Compression threshold: compress values larger than 1KB
    private const int CompressionThreshold = 1024;
    private const string CompressedPrefix = "GZIP:";
    
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    // Retry policy for transient failures
    private readonly AsyncRetryPolicy _retryPolicy;

    public RedisCacheService(
        IDistributedCache cache, 
        IConnectionMultiplexer? redis = null,
        ILogger<RedisCacheService>? logger = null)
    {
        _cache = cache;
        _redis = redis;
        _logger = logger;
        
        // Configure retry policy: 3 retries with exponential backoff
        _retryPolicy = Policy
            .Handle<RedisConnectionException>()
            .Or<RedisTimeoutException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger?.LogWarning(
                        exception,
                        "Redis retry {RetryCount} after {Delay}ms for operation {Operation}",
                        retryCount, timeSpan.TotalMilliseconds, context.OperationKey);
                });
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        return await _retryPolicy.ExecuteAsync(async (context) =>
        {
            var data = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(data))
            {
                _logger?.LogDebug("Cache MISS for key: {Key}", key);
                return default;
            }

            _logger?.LogDebug("Cache HIT for key: {Key}", key);
            
            // Check if data is compressed
            var jsonData = data.StartsWith(CompressedPrefix) 
                ? Decompress(data[CompressedPrefix.Length..]) 
                : data;

            return JsonSerializer.Deserialize<T>(jsonData, _serializerOptions);
        }, new Context(nameof(GetAsync)));
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        await _retryPolicy.ExecuteAsync(async (context) =>
        {
            var json = JsonSerializer.Serialize(value, _serializerOptions);
            
            // Compress large values
            var dataToStore = json.Length > CompressionThreshold 
                ? CompressedPrefix + Compress(json) 
                : json;

            var options = new DistributedCacheEntryOptions();
            if (expiry.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiry;
            }

            await _cache.SetStringAsync(key, dataToStore, options);
            
            _logger?.LogDebug(
                "Cache SET key: {Key}, Size: {OriginalSize}B -> {StoredSize}B, Expiry: {Expiry}",
                key, json.Length, dataToStore.Length, expiry?.ToString() ?? "None");
        }, new Context(nameof(SetAsync)));
    }

    public async Task RemoveAsync(string key)
    {
        await _retryPolicy.ExecuteAsync(async (context) =>
        {
            await _cache.RemoveAsync(key);
            _logger?.LogDebug("Cache REMOVE key: {Key}", key);
        }, new Context(nameof(RemoveAsync)));
    }

    public async Task AddToSetAsync(string key, string value, TimeSpan? expiry = null)
    {
        // Use Redis native SET if available for better performance
        if (_redis != null && _redis.IsConnected)
        {
            await _retryPolicy.ExecuteAsync(async (context) =>
            {
                var db = _redis.GetDatabase();
                await db.SetAddAsync(key, value);
                if (expiry.HasValue)
                {
                    await db.KeyExpireAsync(key, expiry);
                }
            }, new Context(nameof(AddToSetAsync)));
        }
        else
        {
            // Fallback to list-based implementation
            var set = await GetAsync<List<string>>(key) ?? new List<string>();
            if (!set.Contains(value))
                set.Add(value);
            await SetAsync(key, set, expiry);
        }
    }

    public async Task RemoveFromSetAsync(string key, string value)
    {
        // Use Redis native SET if available
        if (_redis != null && _redis.IsConnected)
        {
            await _retryPolicy.ExecuteAsync(async (context) =>
            {
                var db = _redis.GetDatabase();
                await db.SetRemoveAsync(key, value);
            }, new Context(nameof(RemoveFromSetAsync)));
        }
        else
        {
            var set = await GetAsync<List<string>>(key);
            if (set == null) return;
            set.Remove(value);
            await SetAsync(key, set);
        }
    }

    public async Task<IEnumerable<string>> SetMembersAsync(string key)
    {
        // Use Redis native SET if available
        if (_redis != null && _redis.IsConnected)
        {
            return await _retryPolicy.ExecuteAsync(async (context) =>
            {
                var db = _redis.GetDatabase();
                var members = await db.SetMembersAsync(key);
                return members.Select(m => m.ToString()).Where(m => m != null)!;
            }, new Context(nameof(SetMembersAsync)));
        }
        
        var set = await GetAsync<List<string>>(key);
        return set ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// Get or set pattern - reduces cache stampede
    /// </summary>
    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        var cached = await GetAsync<T>(key);
        if (cached != null)
            return cached;

        var value = await factory();
        if (value != null)
            await SetAsync(key, value, expiry);
        
        return value;
    }

    /// <summary>
    /// Batch remove keys by pattern (use with caution)
    /// </summary>
    public async Task RemoveByPatternAsync(string pattern)
    {
        if (_redis == null || !_redis.IsConnected)
        {
            _logger?.LogWarning("Cannot remove by pattern - Redis connection not available");
            return;
        }

        await _retryPolicy.ExecuteAsync(async (context) =>
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var db = _redis.GetDatabase();
            var keys = server.Keys(pattern: pattern).ToArray();
            
            if (keys.Length > 0)
            {
                await db.KeyDeleteAsync(keys);
                _logger?.LogInformation("Removed {Count} keys matching pattern: {Pattern}", keys.Length, pattern);
            }
        }, new Context(nameof(RemoveByPatternAsync)));
    }

    #region Compression Helpers
    
    private static string Compress(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.Fastest))
        {
            gzip.Write(bytes, 0, bytes.Length);
        }
        return Convert.ToBase64String(output.ToArray());
    }

    private static string Decompress(string compressedText)
    {
        var bytes = Convert.FromBase64String(compressedText);
        using var input = new MemoryStream(bytes);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return Encoding.UTF8.GetString(output.ToArray());
    }
    
    #endregion
}
