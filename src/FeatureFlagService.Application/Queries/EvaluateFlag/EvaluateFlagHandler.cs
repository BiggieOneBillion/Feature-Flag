// src/FeatureFlagService.Application/Queries/EvaluateFlag/EvaluateFlagHandler.cs
using FeatureFlagService.Application.Interfaces;
using FeatureFlagService.Application.Options;
using FeatureFlagService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace FeatureFlagService.Application.Queries.EvaluateFlag;

public class EvaluateFlagHandler : IRequestHandler<EvaluateFlagQuery, bool>
{
    private readonly IFeatureFlagCache      _cache;
    private readonly IFeatureFlagRepository _repo;
    private readonly FeatureFlagOptions     _opts;

    public EvaluateFlagHandler(
        IFeatureFlagCache cache,
        IFeatureFlagRepository repo,
        IOptions<FeatureFlagOptions> opts)
    {
        _cache = cache; _repo = repo; _opts = opts.Value;
    }

    public async Task<bool> Handle(EvaluateFlagQuery q, CancellationToken ct)
    {
        // Fast path: try cache first
        var flag = await _cache.GetAsync(q.Key);

        // Slow path: DB fallback, then warm cache
        if (flag is null)
        {
            flag = await _repo.GetByKeyAsync(q.Key);
            if (flag is not null) await _cache.SetAsync(flag);
        }

        if (flag is null)
        {
            return _opts.StrictMode
                ? throw new KeyNotFoundException($"Flag '{q.Key}' not found")
                : false; // default = safe off
        }

        return flag.IsEnabledFor(q.UserId, q.Role);
    }
}
