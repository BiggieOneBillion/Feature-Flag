// src/FeatureFlagService.Domain/Extensions/StringExtensions.cs
namespace FeatureFlagService.Domain.Extensions;

public static class StringExtensions
{
    /// Stable, process-independent hash — safe for percentage rollout.
    public static int GetStableHashCode(this string str)
    {
        unchecked
        {
            int hash = 23;
            foreach (char c in str)
                hash = hash * 31 + c;
            return hash;
        }
    }
}
