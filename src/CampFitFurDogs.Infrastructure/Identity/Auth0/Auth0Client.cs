using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using CampFitFurDogs.Application.Abstractions.Authentication.Auth0;
using CampFitFurDogs.Domain.Errors;

namespace CampFitFurDogs.Infrastructure.Identity.Auth0;

public class Auth0Client : IAuth0Client
{
    private readonly HttpClient _http;

    public Auth0Client(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> ExchangeCodeForTokenAsync(
        string code,
        string clientId,
        string clientSecret,
        string callbackUrl,
        string domain,
        CancellationToken ct)
    {
        var tokenEndpoint = $"https://{domain}/oauth/token";

        var request = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["code"] = code,
            ["redirect_uri"] = callbackUrl
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

        return token;
    }

    public async Task<Auth0UserInfo> GetUserInfoAsync(
        string accessToken,
        string domain,
        CancellationToken ct)
    {
        var endpoint = $"https://{domain}/userinfo";

        using var req = new HttpRequestMessage(HttpMethod.Get, endpoint);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

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

        return new Auth0UserInfo(sub, first, last, email);
    }
}
