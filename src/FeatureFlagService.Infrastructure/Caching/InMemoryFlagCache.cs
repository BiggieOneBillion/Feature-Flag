// src/FeatureFlagService.Infrastructure/Caching/InMemoryFlagCache.cs
using FeatureFlagService.Application.Interfaces;
using FeatureFlagService.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace FeatureFlagService.Infrastructure.Caching;

public class InMemoryFlagCache : IFeatureFlagCache
{
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan TTL = TimeSpan.FromMinutes(5);

    public InMemoryFlagCache(IMemoryCache cache) => _cache = cache;

    private static string CacheKey(string key) => $"flag:{key}";

    public Task<FeatureFlag?> GetAsync(string key) =>
        Task.FromResult(_cache.TryGetValue(CacheKey(key), out FeatureFlag? f) ? f : null);

    public Task SetAsync(FeatureFlag flag)
    {
        _cache.Set(CacheKey(flag.Key), flag, TTL);
        return Task.CompletedTask;
    }

    public Task InvalidateAsync(string key)
    {
        _cache.Remove(CacheKey(key));
        return Task.CompletedTask;
    }
}
