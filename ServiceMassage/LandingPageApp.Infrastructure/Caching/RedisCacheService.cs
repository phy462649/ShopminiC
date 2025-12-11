// File: Caching/RedisCacheService.cs
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using LandingPageApp.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LandingPageApp.Infrastructure.Caching;

public class RedisCacheService : ICacheRediservice
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(data))
            return default;

        return JsonSerializer.Deserialize<T>(data, _serializerOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value, _serializerOptions);

        var options = new DistributedCacheEntryOptions();
        if (expiry.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiry;
        }

        await _cache.SetStringAsync(key, json, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
    public async Task AddToSetAsync(string key, string value, TimeSpan? expiry = null)
    {
        var set = await GetAsync<List<string>>(key) ?? new List<string>();
        if (!set.Contains(value))
            set.Add(value);

        await SetAsync(key, set, expiry);
    }

    public async Task RemoveFromSetAsync(string key, string value)
    {
        var set = await GetAsync<List<string>>(key);
        if (set == null) return;

        set.Remove(value);
        await SetAsync(key, set);
    }

    public async Task<IEnumerable<string>> SetMembersAsync(string key)
    {
        var set = await GetAsync<List<string>>(key);
        return set ?? Enumerable.Empty<string>();
    }
}
