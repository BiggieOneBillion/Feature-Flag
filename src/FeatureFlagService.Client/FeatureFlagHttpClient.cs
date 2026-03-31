// src/FeatureFlagService.Client/FeatureFlagHttpClient.cs
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace FeatureFlagService.Client;

public class FeatureFlagHttpClient : IFeatureFlagClient
{
    private readonly HttpClient            _http;
    private readonly FeatureFlagClientOptions _opts;

    public FeatureFlagHttpClient(HttpClient http, IOptions<FeatureFlagClientOptions> opts)
    { _http = http; _opts = opts.Value; }

    public async Task<bool> IsEnabledAsync(string flagKey, string userId, string role = "")
    {
        try
        {
            var url = $"api/flags/{flagKey}/evaluate?userId={userId}&role={role}";
            var result = await _http.GetFromJsonAsync<EvaluateResponse>(url);
            return result?.Enabled ?? false;
        }
        catch
        {
            // Fail safe: if flag service is down, features default to OFF
            return false;
        }
    }

    private record EvaluateResponse(string Key, string UserId, bool Enabled);
}
