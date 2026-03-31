// src/FeatureFlagService.Application/DTOs/FlagDto.cs
namespace FeatureFlagService.Application.DTOs;

public record FlagDto(
    Guid Id,
    string Key,
    bool IsEnabled,
    List<string> AllowedUserIds,
    List<string> AllowedRoles,
    int RolloutPercentage,
    DateTime UpdatedAt
);
