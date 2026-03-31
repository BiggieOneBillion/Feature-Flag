// src/FeatureFlagService.API/Program.cs
using FeatureFlagService.Application.Commands.UpsertFlag;
using FeatureFlagService.Application.Interfaces;
using FeatureFlagService.Application.Options;
using FeatureFlagService.Domain.Interfaces;
using FeatureFlagService.Infrastructure.Caching;
using FeatureFlagService.Infrastructure.Persistence;
using FeatureFlagService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Configuration ────────────────────────────────────────────────────
builder.Services.Configure<FeatureFlagOptions>(
    builder.Configuration.GetSection("FeatureFlags"));

// ── Caching (swap line below to switch In-Memory ↔ Redis) ─────────
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IFeatureFlagCache, InMemoryFlagCache>();
// Phase 2: comment above two lines, uncomment below:
// builder.Services.AddStackExchangeRedisCache(o =>
//     o.Configuration = builder.Configuration.GetConnectionString("Redis"));
// builder.Services.AddSingleton<IFeatureFlagCache, RedisFlagCache>();

// ── Repositories ─────────────────────────────────────────────────────
builder.Services.AddScoped<IFeatureFlagRepository, FeatureFlagRepository>();

// ── MediatR ──────────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(UpsertFlagCommand).Assembly));

// ── Web API ──────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();

app.Run();
