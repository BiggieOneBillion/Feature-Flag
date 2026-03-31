// src/FeatureFlagService.Application/Queries/GetAllFlags/GetAllFlagsHandler.cs
using FeatureFlagService.Application.DTOs;
using FeatureFlagService.Domain.Interfaces;
using MediatR;

namespace FeatureFlagService.Application.Queries.GetAllFlags;

public class GetAllFlagsHandler : IRequestHandler<GetAllFlagsQuery, IEnumerable<FlagDto>>
{
    private readonly IFeatureFlagRepository _repo;

    public GetAllFlagsHandler(IFeatureFlagRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<FlagDto>> Handle(GetAllFlagsQuery request, CancellationToken cancellationToken)
    {
        var flags = await _repo.GetAllAsync();
        return flags.Select(f => new FlagDto(
            f.Id,
            f.Key,
            f.IsEnabled,
            f.AllowedUserIds,
            f.AllowedRoles,
            f.RolloutPercentage,
            f.UpdatedAt
        ));
    }
}
