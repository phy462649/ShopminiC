// File: Caching/RedisCacheService.cs
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using LandingPageApp.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace LandingPageApp.Infrastructure.Caching;

public class RedisCacheService : ICacheRediservice
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer? _redis;
    private static readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "redis_log.txt");
    private static readonly object _lockObj = new();
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer? redis = null)
    {
        _cache = cache;
        _redis = redis;
    }

    private async Task LogOperationAndDumpAllDataAsync(string operation, string key, string? value = null, string? result = null)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\n{'=',0}{'=',60}");
            sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}]");
            sb.AppendLine($"OPERATION: {operation}");
            sb.AppendLine($"KEY: {key}");
            if (!string.IsNullOrEmpty(value))
                sb.AppendLine($"VALUE: {value}");
            if (!string.IsNullOrEmpty(result))
                sb.AppendLine($"RESULT: {result}");
            sb.AppendLine($"{'=',0}{'=',60}");

            // Dump all Redis data
            sb.AppendLine("\n--- ALL REDIS DATA ---");
            
            if (_redis != null && _redis.IsConnected)
            {
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var db = _redis.GetDatabase();
                var keys = server.Keys(pattern: "*").ToList();
                
                sb.AppendLine($"Total Keys: {keys.Count}");
                sb.AppendLine();

                foreach (var redisKey in keys)
                {
                    try
                    {
                        var keyType = await db.KeyTypeAsync(redisKey);
                        var ttl = await db.KeyTimeToLiveAsync(redisKey);
                        var ttlStr = ttl.HasValue ? ttl.Value.ToString(@"d\.hh\:mm\:ss") : "No expiry";

                        sb.AppendLine($"  Key: {redisKey}");
                        sb.AppendLine($"  Type: {keyType}");
                        sb.AppendLine($"  TTL: {ttlStr}");

                        switch (keyType)
                        {
                            case RedisType.String:
                                var strValue = await db.StringGetAsync(redisKey);
                                sb.AppendLine($"  Value: {strValue}");
                                break;
                            case RedisType.List:
                                var listValues = await db.ListRangeAsync(redisKey);
                                sb.AppendLine($"  Values: [{string.Join(", ", listValues.Select(v => v.ToString()))}]");
                                break;
                            case RedisType.Set:
                                var setValues = await db.SetMembersAsync(redisKey);
                                sb.AppendLine($"  Values: [{string.Join(", ", setValues.Select(v => v.ToString()))}]");
                                break;
                            case RedisType.Hash:
                                var hashValues = await db.HashGetAllAsync(redisKey);
                                sb.AppendLine($"  Values: {{{string.Join(", ", hashValues.Select(h => $"{h.Name}:{h.Value}"))}}}");
                                break;
                            default:
                                sb.AppendLine($"  Value: (unsupported type)");
                                break;
                        }
                        sb.AppendLine();
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"  Error reading key {redisKey}: {ex.Message}");
                        sb.AppendLine();
                    }
                }
            }
            else
            {
                sb.AppendLine("Redis connection not available for key enumeration.");
                sb.AppendLine("Using IDistributedCache (cannot list all keys).");
            }

            sb.AppendLine("--- END REDIS DATA ---\n");

            lock (_lockObj)
            {
                File.AppendAllText(_logFilePath, sb.ToString());
            }
        }
        catch (Exception ex)
        {
            lock (_lockObj)
            {
                File.AppendAllText(_logFilePath, $"[{DateTime.Now}] LOG ERROR: {ex.Message}\n");
            }
        }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        var result = string.IsNullOrEmpty(data) ? "NULL" : data;
        
        // Only log for GET operations (not internal calls)
        // await LogOperationAndDumpAllDataAsync("GET", key, null, result);

        if (string.IsNullOrEmpty(data))
            return default;

        return JsonSerializer.Deserialize<T>(data, _serializerOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value, _serializerOptions);
        var expiryStr = expiry.HasValue ? expiry.Value.ToString() : "No expiry";

        var options = new DistributedCacheEntryOptions();
        if (expiry.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiry;
        }

        await _cache.SetStringAsync(key, json, options);
        
        // Log after SET
        await LogOperationAndDumpAllDataAsync("SET", key, json, $"Expiry: {expiryStr}");
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
        
        // Log after REMOVE
        await LogOperationAndDumpAllDataAsync("REMOVE", key);
    }

    public async Task AddToSetAsync(string key, string value, TimeSpan? expiry = null)
    {
        var set = await GetAsync<List<string>>(key) ?? new List<string>();
        if (!set.Contains(value))
            set.Add(value);

        await SetAsync(key, set, expiry);
        // SetAsync already logs
    }

    public async Task RemoveFromSetAsync(string key, string value)
    {
        var set = await GetAsync<List<string>>(key);
        if (set == null) return;

        set.Remove(value);
        await SetAsync(key, set);
        // SetAsync already logs
    }

    public async Task<IEnumerable<string>> SetMembersAsync(string key)
    {
        var set = await GetAsync<List<string>>(key);
        return set ?? Enumerable.Empty<string>();
    }
}
