// src/FeatureFlagService.Application/Commands/DeleteFlag/DeleteFlagHandler.cs
using FeatureFlagService.Application.Interfaces;
using FeatureFlagService.Domain.Interfaces;
using MediatR;

namespace FeatureFlagService.Application.Commands.DeleteFlag;

public class DeleteFlagHandler : IRequestHandler<DeleteFlagCommand>
{
    private readonly IFeatureFlagRepository _repo;
    private readonly IFeatureFlagCache      _cache;

    public DeleteFlagHandler(IFeatureFlagRepository repo, IFeatureFlagCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task Handle(DeleteFlagCommand cmd, CancellationToken ct)
    {
        await _repo.DeleteAsync(cmd.Key);
        await _cache.InvalidateAsync(cmd.Key);
    }
}
