// tests/FeatureFlagService.Tests/Domain/FeatureFlagTests.cs
using FeatureFlagService.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace FeatureFlagService.Tests.Domain;

public class FeatureFlagTests
{
    [Fact]
    public void IsEnabledFor_DisabledFlag_ReturnsFalse()
    {
        var flag = FeatureFlag.Create("beta", isEnabled: false, new List<string>(), new List<string>(), 100);
        flag.IsEnabledFor("admin", "admin").Should().BeFalse();
    }

    [Fact]
    public void IsEnabledFor_AllowedUser_ReturnsTrue()
    {
        var flag = FeatureFlag.Create("beta", true, new List<string> { "user-42" }, new List<string>(), 0);
        flag.IsEnabledFor("user-42", "").Should().BeTrue();
    }

    [Fact]
    public void IsEnabledFor_RolloutPercentage_IsDeterministic()
    {
        var flag = FeatureFlag.Create("rollout", true, new List<string>(), new List<string>(), 50);
        var first  = flag.IsEnabledFor("user-99", "");
        var second = flag.IsEnabledFor("user-99", "");
        first.Should().Be(second);  // same user always same result
    }

    [Theory]
    [InlineData(0,   false)]
    [InlineData(100, true)]
    public void IsEnabledFor_EdgeRolloutPercentages(int pct, bool expected)
    {
        var flag = FeatureFlag.Create("edge", true, new List<string>(), new List<string>(), pct);
        // At 0% nobody gets it; at 100% everyone does
        // (user-x hashes to the same bucket every time)
        flag.IsEnabledFor("user-x", "").Should().Be(expected);
    }
}
