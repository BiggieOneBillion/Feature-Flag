// src/FeatureFlagService.Client/ServiceCollectionExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace FeatureFlagService.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFeatureFlagClient(
        this IServiceCollection services,
        Action<FeatureFlagClientOptions> configure)
    {
        services.Configure(configure);

        services.AddHttpClient<IFeatureFlagClient, FeatureFlagHttpClient>((sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptions<FeatureFlagClientOptions>>().Value;
            client.BaseAddress = new Uri(opts.BaseUrl);
            client.Timeout     = TimeSpan.FromSeconds(opts.TimeoutSeconds);
            if (!string.IsNullOrEmpty(opts.ApiKey))
                client.DefaultRequestHeaders.Add("X-Api-Key", opts.ApiKey);
        })
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(2, retry => TimeSpan.FromMilliseconds(100 * retry)))
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30)));

        return services;
    }
}
