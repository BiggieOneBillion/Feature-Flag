// src/FeatureFlagService.Application/Commands/UpsertFlag/UpsertFlagHandler.cs
using FeatureFlagService.Application.Interfaces;
using FeatureFlagService.Domain.Entities;
using FeatureFlagService.Domain.Interfaces;
using MediatR;

namespace FeatureFlagService.Application.Commands.UpsertFlag;

public class UpsertFlagHandler : IRequestHandler<UpsertFlagCommand, Guid>
{
    private readonly IFeatureFlagRepository _repo;
    private readonly IFeatureFlagCache      _cache;

    public UpsertFlagHandler(IFeatureFlagRepository repo, IFeatureFlagCache cache)
    { _repo = repo; _cache = cache; }

    public async Task<Guid> Handle(UpsertFlagCommand cmd, CancellationToken ct)
    {
        var existing = await _repo.GetByKeyAsync(cmd.Key);

        if (existing is null)
        {
            var flag = FeatureFlag.Create(cmd.Key, cmd.IsEnabled,
                cmd.AllowedUserIds, cmd.AllowedRoles, cmd.RolloutPercentage);
            await _repo.AddAsync(flag);
            await _cache.InvalidateAsync(cmd.Key);
            return flag.Id;
        }

        existing.Update(cmd.IsEnabled, cmd.AllowedUserIds,
                        cmd.AllowedRoles, cmd.RolloutPercentage);
        await _repo.UpdateAsync(existing);
        await _cache.InvalidateAsync(cmd.Key);  // always invalidate on write
        return existing.Id;
    }
}
