// src/FeatureFlagService.Infrastructure/Persistence/AppDbContext.cs
using System.Text.Json;
using FeatureFlagService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeatureFlagService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<FeatureFlag>(e =>
        {
            e.HasKey(f => f.Id);
            e.HasIndex(f => f.Key).IsUnique();
            e.Property(f => f.Key).IsRequired().HasMaxLength(200);

            // Serialize lists as JSON columns (Postgres JSONB or TEXT)
            var opts = JsonSerializerOptions.Default;
            e.Property(f => f.AllowedUserIds)
             .HasConversion(
                v => JsonSerializer.Serialize(v, opts),
                v => JsonSerializer.Deserialize<List<string>>(v, opts)!);
            e.Property(f => f.AllowedRoles)
             .HasConversion(
                v => JsonSerializer.Serialize(v, opts),
                v => JsonSerializer.Deserialize<List<string>>(v, opts)!);
        });
    }
}
