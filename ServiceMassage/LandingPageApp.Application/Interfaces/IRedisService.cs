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
}
