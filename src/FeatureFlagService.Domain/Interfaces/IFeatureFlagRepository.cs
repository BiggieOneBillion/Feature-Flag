// src/FeatureFlagService.Domain/Interfaces/IFeatureFlagRepository.cs
using FeatureFlagService.Domain.Entities;

namespace FeatureFlagService.Domain.Interfaces;

public interface IFeatureFlagRepository
{
    Task<FeatureFlag?>              GetByKeyAsync(string key);
    Task<IEnumerable<FeatureFlag>>  GetAllAsync();
    Task                            AddAsync(FeatureFlag flag);
    Task                            UpdateAsync(FeatureFlag flag);
    Task                            DeleteAsync(string key);
}
