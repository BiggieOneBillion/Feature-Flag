// src/FeatureFlagService.Infrastructure/Caching/RedisFlagCache.cs
using System.Text.Json;
using FeatureFlagService.Application.Interfaces;
using FeatureFlagService.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace FeatureFlagService.Infrastructure.Caching;

public class RedisFlagCache : IFeatureFlagCache
{
    private readonly IDistributedCache _redis;
    private static readonly TimeSpan TTL = TimeSpan.FromMinutes(5);

    public RedisFlagCache(IDistributedCache redis) => _redis = redis;

    private static string CacheKey(string key) => $"flag:{key}";

    public async Task<FeatureFlag?> GetAsync(string key)
    {
        var json = await _redis.GetStringAsync(CacheKey(key));
        return json is null ? null : JsonSerializer.Deserialize<FeatureFlag>(json);
    }

    public async Task SetAsync(FeatureFlag flag)
    {
        var json = JsonSerializer.Serialize(flag);
        await _redis.SetStringAsync(CacheKey(flag.Key), json,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TTL });
    }

    public async Task InvalidateAsync(string key) =>
        await _redis.RemoveAsync(CacheKey(key));
}
