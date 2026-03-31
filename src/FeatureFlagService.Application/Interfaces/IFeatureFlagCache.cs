// src/FeatureFlagService.Application/Interfaces/IFeatureFlagCache.cs
using FeatureFlagService.Domain.Entities;

namespace FeatureFlagService.Application.Interfaces;

public interface IFeatureFlagCache
{
    Task<FeatureFlag?> GetAsync(string key);
    Task               SetAsync(FeatureFlag flag);
    Task               InvalidateAsync(string key);
}
