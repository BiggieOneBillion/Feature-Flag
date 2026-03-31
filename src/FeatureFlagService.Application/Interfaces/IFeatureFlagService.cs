// src/FeatureFlagService.Application/Interfaces/IFeatureFlagService.cs
namespace FeatureFlagService.Application.Interfaces;

public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(string flagKey, string userId, string role);
    Task<bool> IsEnabledAsync(string flagKey, string userId);  // role-less overload
}
