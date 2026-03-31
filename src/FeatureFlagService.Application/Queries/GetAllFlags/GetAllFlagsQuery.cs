// src/FeatureFlagService.Application/Queries/GetAllFlags/GetAllFlagsQuery.cs
using FeatureFlagService.Application.DTOs;
using MediatR;

namespace FeatureFlagService.Application.Queries.GetAllFlags;

public record GetAllFlagsQuery() : IRequest<IEnumerable<FlagDto>>;
