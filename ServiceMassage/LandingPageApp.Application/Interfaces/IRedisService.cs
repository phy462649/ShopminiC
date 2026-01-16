// File: Caching/ICacheService.cs
using System;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Interfaces;

public interface ICacheRediservice
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task<IEnumerable<string>> SetMembersAsync(string key);
    Task AddToSetAsync(string key, string value, TimeSpan? expiry = null);
    Task RemoveFromSetAsync(string key, string value);
    
    /// <summary>
    /// Get or set pattern - reduces cache stampede
    /// </summary>
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null);
    
    /// <summary>
    /// Batch remove keys by pattern
    /// </summary>
    Task RemoveByPatternAsync(string pattern);
}
