// src/FeatureFlagService.Application/Commands/UpsertFlag/UpsertFlagCommand.cs
using MediatR;

namespace FeatureFlagService.Application.Commands.UpsertFlag;

public record UpsertFlagCommand(
    string       Key,
    bool         IsEnabled,
    List<string> AllowedUserIds,
    List<string> AllowedRoles,
    int          RolloutPercentage
) : IRequest<Guid>;
