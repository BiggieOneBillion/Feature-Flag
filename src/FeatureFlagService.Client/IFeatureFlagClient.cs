// src/FeatureFlagService.Client/IFeatureFlagClient.cs
namespace FeatureFlagService.Client;

public interface IFeatureFlagClient
{
    Task<bool> IsEnabledAsync(string flagKey, string userId, string role = "");
}
