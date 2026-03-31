// src/FeatureFlagService.Client/FeatureFlagClientOptions.cs
namespace FeatureFlagService.Client;

public class FeatureFlagClientOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey  { get; set; } = string.Empty;  // optional auth header
    public int    TimeoutSeconds { get; set; } = 2;       // keep it tight — flags must be fast
}
