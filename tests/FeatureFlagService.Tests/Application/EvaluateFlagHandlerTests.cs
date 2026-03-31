// tests/FeatureFlagService.Tests/Application/EvaluateFlagHandlerTests.cs
using FeatureFlagService.Application.Interfaces;
using FeatureFlagService.Application.Queries.EvaluateFlag;
using FeatureFlagService.Application.Options;
using FeatureFlagService.Domain.Entities;
using FeatureFlagService.Domain.Interfaces;
using Moq;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.Options;

namespace FeatureFlagService.Tests.Application;

public class EvaluateFlagHandlerTests
{
    private readonly Mock<IOptions<FeatureFlagOptions>> _optionsMock;

    public EvaluateFlagHandlerTests()
    {
        _optionsMock = new Mock<IOptions<FeatureFlagOptions>>();
        _optionsMock.Setup(o => o.Value).Returns(new FeatureFlagOptions());
    }

    [Fact]
    public async Task Handle_CacheHit_DoesNotHitRepository()
    {
        var flag    = FeatureFlag.Create("dark-mode", true, new List<string>(), new List<string> { "admin" }, 0);
        var cache   = new Mock<IFeatureFlagCache>();
        var repo    = new Mock<IFeatureFlagRepository>();

        cache.Setup(c => c.GetAsync("dark-mode")).ReturnsAsync(flag);

        var handler = new EvaluateFlagHandler(cache.Object, repo.Object, _optionsMock.Object);
        var result  = await handler.Handle(
            new EvaluateFlagQuery("dark-mode", "u1", "admin"), default);

        result.Should().BeTrue();
        repo.Verify(r => r.GetByKeyAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CacheMiss_WarmsCacheFromRepo()
    {
        var flag  = FeatureFlag.Create("new-ui", true, new List<string> { "u1" }, new List<string>(), 0);
        var cache = new Mock<IFeatureFlagCache>();
        var repo  = new Mock<IFeatureFlagRepository>();

        cache.Setup(c => c.GetAsync("new-ui")).ReturnsAsync((FeatureFlag?)null);
        repo.Setup(r => r.GetByKeyAsync("new-ui")).ReturnsAsync(flag);

        var handler = new EvaluateFlagHandler(cache.Object, repo.Object, _optionsMock.Object);
        await handler.Handle(new EvaluateFlagQuery("new-ui", "u1", ""), default);

        cache.Verify(c => c.SetAsync(flag), Times.Once);
    }
}
