// src/FeatureFlagService.Application/Commands/DeleteFlag/DeleteFlagCommand.cs
using MediatR;

namespace FeatureFlagService.Application.Commands.DeleteFlag;

public record DeleteFlagCommand(string Key) : IRequest;
