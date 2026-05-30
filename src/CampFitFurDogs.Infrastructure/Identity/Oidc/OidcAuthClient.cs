using System.Net.Http.Headers;
using System.Text.Json;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Domain.Errors;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.Infrastructure.Identity.Oidc;

public sealed class OidcAuthClient : IAuthClient
{
    private readonly HttpClient _http;
    private readonly OidcOptions _options;

    public OidcAuthClient(HttpClient http, IOptions<OidcOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public async Task<AuthToken> ExchangeAsync(string authorizationCode, CancellationToken ct)
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
            throw new ExternalAuthProviderException("Failed to exchange authorization code.");

        var json = await response.Content.ReadAsStringAsync(ct);
        var root = JsonSerializer.Deserialize<JsonElement>(json);

        var token = root.GetProperty("access_token").GetString();
        if (string.IsNullOrWhiteSpace(token))
            throw new ExternalAuthProviderException("Missing access token.");

        return new AuthToken(token);
    }

    public async Task<AuthUser> GetUserAsync(AuthToken token, CancellationToken ct)
    {
        var endpoint = $"https://{_options.Authority}/userinfo";

        using var req = new HttpRequestMessage(HttpMethod.Get, endpoint);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var response = await _http.SendAsync(req, ct);
        if (!response.IsSuccessStatusCode)
            throw new ExternalAuthProviderException("Failed to retrieve user profile.");

        var json = await response.Content.ReadAsStringAsync(ct);
        var root = JsonSerializer.Deserialize<JsonElement>(json);

        var sub = root.GetProperty("sub").GetString();
        var first = root.GetProperty("given_name").GetString();
        var last = root.GetProperty("family_name").GetString();
        var email = root.GetProperty("email").GetString();

        if (string.IsNullOrWhiteSpace(sub))
            throw new ExternalAuthProviderException("Missing Auth0 user identifier.");
        if (string.IsNullOrWhiteSpace(first))
            throw new ExternalAuthProviderException("Missing Auth0 first name.");
        if (string.IsNullOrWhiteSpace(last))
            throw new ExternalAuthProviderException("Missing Auth0 last name.");
        if (string.IsNullOrWhiteSpace(email))
            throw new ExternalAuthProviderException("Missing Auth0 email.");

        return new AuthUser(sub, first, last, email);
    }
}
