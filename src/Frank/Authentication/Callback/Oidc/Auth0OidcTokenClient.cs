using System.Net.Http.Headers;
using System.Text.Json;

namespace Frank.Authentication.Callback.Oidc;

public sealed class Auth0OidcTokenClient : IOidcTokenClient
{
    private readonly HttpClient _http;
    private readonly OidcAuthCallbackOptions _options;

    public Auth0OidcTokenClient(HttpClient http, OidcAuthCallbackOptions options)
    {
        _http = http;
        _options = options;
    }

    public async Task<string> ExchangeCodeAsync(string authorizationCode, CancellationToken ct)
    {
        var tokenEndpoint = $"https://{_options.Authority}/oauth/token";

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
            throw new InvalidOperationException("Failed to exchange authorization code.");

        var json = await response.Content.ReadAsStringAsync(ct);
        var root = JsonSerializer.Deserialize<JsonElement>(json);

        var token = root.GetProperty("access_token").GetString();
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Missing access token.");

        return token;
    }
}
