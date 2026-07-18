using System.Net.Http.Headers;
using System.Text.Json;
using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Settings;
using Microsoft.Extensions.Options;

namespace Frank.Identity.Application.Callback.Auth0;

public sealed class Auth0OidcUserInfoClient : IOidcUserInfoClient
{
    private readonly HttpClient _http;
    private readonly OidcCallbackSettings _options;

    public Auth0OidcUserInfoClient(HttpClient http, IOptionsMonitor<OidcCallbackSettings> options)
    {
        _http = http;
        _options = options.CurrentValue;
    }

    public async Task<OidcUserInfo> GetUserInfoAsync(string accessToken, CancellationToken ct)
    {
        var endpoint = $"{_options.Authority}/userinfo";

        using var req = new HttpRequestMessage(HttpMethod.Get, endpoint);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _http.SendAsync(req, ct);

        if (!response.IsSuccessStatusCode)
            throw new OidcProtocolException("Failed to retrieve userinfo from Auth0.");

        var json = await response.Content.ReadAsStringAsync(ct);
        var root = JsonSerializer.Deserialize<JsonElement>(json);

        return new OidcUserInfo(
            Subject: root.GetProperty("sub").GetString()!,
            Email: root.TryGetProperty("email", out var email) ? email.GetString() : null,
            GivenName: root.TryGetProperty("given_name", out var given) ? given.GetString() : null,
            FamilyName: root.TryGetProperty("family_name", out var family) ? family.GetString() : null,
            Picture: root.TryGetProperty("picture", out var pic) ? pic.GetString() : null,
            Claims: ExtractClaims(root)
        );
    }

    private static IReadOnlyDictionary<string, string> ExtractClaims(JsonElement root)
    {
        var dict = new Dictionary<string, string>();

        foreach (var prop in root.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.String)
                dict[prop.Name] = prop.Value.GetString()!;
        }

        return dict;
    }
}
