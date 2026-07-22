using System.Text.Json;
using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Settings;
using Microsoft.Extensions.Options;

namespace Frank.Identity.Infrastructure.Auth0;

public sealed class Auth0OidcTokenClient : IOidcTokenClient
{
    private readonly HttpClient _http;
    private readonly OidcCallbackSettings _options;

    public Auth0OidcTokenClient(HttpClient http, IOptionsMonitor<OidcCallbackSettings> optionsMonitor)
    {
        _http = http;
        _options = optionsMonitor.CurrentValue;
    }

    public async Task<OidcTokenResponse> ExchangeCodeAsync(string authorizationCode, CancellationToken ct)
    {
        var tokenEndpoint = $"{_options.Authority}/oauth/token";

        var request = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["code"] = authorizationCode,
            ["redirect_uri"] = _options.CallbackUrl
        };

        var response = await _http.PostAsync(
            tokenEndpoint,
            new FormUrlEncodedContent(request),
            ct);

        if (!response.IsSuccessStatusCode)
            throw new OidcProtocolException("Failed to exchange authorization code.");

        var json = await response.Content.ReadAsStringAsync(ct);
        var root = JsonSerializer.Deserialize<JsonElement>(json);

        var accessToken = root.TryGetProperty("access_token", out var at)
            ? at.GetString()
            : null;

        var idToken = root.TryGetProperty("id_token", out var id)
            ? id.GetString()
            : null;

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new OidcProtocolException("Missing access token.");

        return new OidcTokenResponse(accessToken, idToken);
    }
}
