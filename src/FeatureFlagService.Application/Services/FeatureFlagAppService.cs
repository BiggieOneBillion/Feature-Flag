// src/FeatureFlagService.Application/Services/FeatureFlagAppService.cs
using FeatureFlagService.Application.Interfaces;
using FeatureFlagService.Application.Queries.EvaluateFlag;
using MediatR;

namespace FeatureFlagService.Application.Services;

public class FeatureFlagAppService : IFeatureFlagService
{
    private readonly IMediator _mediator;
    public FeatureFlagAppService(IMediator mediator) => _mediator = mediator;

    public Task<bool> IsEnabledAsync(string key, string userId, string role) =>
        _mediator.Send(new EvaluateFlagQuery(key, userId, role));

    public Task<bool> IsEnabledAsync(string key, string userId) =>
        IsEnabledAsync(key, userId, string.Empty);
}
