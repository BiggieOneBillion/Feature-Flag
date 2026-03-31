// src/FeatureFlagService.Infrastructure/Persistence/Repositories/FeatureFlagRepository.cs
using FeatureFlagService.Domain.Entities;
using FeatureFlagService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FeatureFlagService.Infrastructure.Persistence.Repositories;

public class FeatureFlagRepository : IFeatureFlagRepository
{
    private readonly AppDbContext _db;
    public FeatureFlagRepository(AppDbContext db) => _db = db;

    public Task<FeatureFlag?> GetByKeyAsync(string key) =>
        _db.FeatureFlags.FirstOrDefaultAsync(f => f.Key == key);

    public Task<IEnumerable<FeatureFlag>> GetAllAsync() =>
        _db.FeatureFlags.ToListAsync().ContinueWith(t => t.Result.AsEnumerable());

    public async Task AddAsync(FeatureFlag flag)
    {
        _db.FeatureFlags.Add(flag);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(FeatureFlag flag)
    {
        _db.FeatureFlags.Update(flag);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string key)
    {
        var flag = await GetByKeyAsync(key);
        if (flag is not null) { _db.FeatureFlags.Remove(flag); await _db.SaveChangesAsync(); }
    }
}
