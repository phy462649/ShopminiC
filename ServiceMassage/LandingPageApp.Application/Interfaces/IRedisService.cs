// File: Caching/ICacheService.cs
using System;
using System.Threading.Tasks;

namespace LandingPageApp.Infrastructure.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
}
