// src/FeatureFlagService.Domain/Entities/FeatureFlag.cs
using FeatureFlagService.Domain.Extensions;

namespace FeatureFlagService.Domain.Entities;

public class FeatureFlag
{
    public Guid   Id                 { get; private set; }
    public string Key                { get; private set; } = string.Empty;
    public bool   IsEnabled          { get; private set; }
    public List<string> AllowedUserIds { get; private set; } = new();
    public List<string> AllowedRoles   { get; private set; } = new();
    public int    RolloutPercentage  { get; private set; }   // 0 – 100
    public DateTime UpdatedAt        { get; private set; }

    // EF Core needs a parameterless constructor
    private FeatureFlag() { }

    public static FeatureFlag Create(
        string key, bool isEnabled,
        List<string> userIds, List<string> roles, int rollout)
    {
        return new FeatureFlag
        {
            Id                = Guid.NewGuid(),
            Key               = key,
            IsEnabled         = isEnabled,
            AllowedUserIds    = userIds,
            AllowedRoles      = roles,
            RolloutPercentage = Math.Clamp(rollout, 0, 100),
            UpdatedAt         = DateTime.UtcNow
        };
    }

    public void Update(bool isEnabled, List<string> userIds,
                       List<string> roles, int rollout)
    {
        IsEnabled         = isEnabled;
        AllowedUserIds    = userIds;
        AllowedRoles      = roles;
        RolloutPercentage = Math.Clamp(rollout, 0, 100);
        UpdatedAt         = DateTime.UtcNow;
    }

    // --- Core evaluation logic lives here, not in handlers ---
    public bool IsEnabledFor(string userId, string role)
    {
        if (!IsEnabled) return false;
        if (AllowedUserIds.Contains(userId)) return true;
        if (AllowedRoles.Contains(role))     return true;

        if (RolloutPercentage > 0)
        {
            // Deterministic: same user always lands in the same bucket
            var hash = Math.Abs($"{Key}:{userId}".GetStableHashCode());
            return (hash % 100) < RolloutPercentage;
        }

        return false;
    }
}
