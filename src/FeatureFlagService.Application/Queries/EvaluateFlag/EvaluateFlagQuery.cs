// src/FeatureFlagService.Application/Queries/EvaluateFlag/EvaluateFlagQuery.cs
using MediatR;

namespace FeatureFlagService.Application.Queries.EvaluateFlag;

public record EvaluateFlagQuery(string Key, string UserId, string Role)
    : IRequest<bool>;
