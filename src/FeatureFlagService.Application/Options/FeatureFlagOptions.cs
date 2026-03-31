// src/FeatureFlagService.Application/Options/FeatureFlagOptions.cs
namespace FeatureFlagService.Application.Options;

public class FeatureFlagOptions
{
    public int  CacheTtlMinutes      { get; set; } = 5;
    public int  DefaultRolloutPct    { get; set; } = 0;
    public bool EnableAuditLogging   { get; set; } = true;
    public bool StrictMode           { get; set; } = false; // false = missing flag => disabled
}
