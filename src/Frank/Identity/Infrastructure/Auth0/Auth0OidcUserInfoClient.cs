using System.Net.Http.Headers;
using System.Text.Json;
using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Frank.Identity.Infrastructure.Auth0;

/// <summary>
/// Auth0 implementation of <see cref="IOidcUserInfoClient"/> responsible for
/// retrieving user profile information from the OIDC <c>/userinfo</c> endpoint.
/// <para>
/// This client performs a bearer‑authenticated GET request to Auth0’s UserInfo
/// endpoint and maps the JSON response into a structured <see cref="OidcUserInfo"/>
/// object. The class supports dynamic configuration reloads via
/// <see cref="IOptionsMonitor{T}"/>.
/// </para>
/// </summary>
public sealed class Auth0OidcUserInfoClient : IOidcUserInfoClient
{
    private readonly HttpClient _http;
    private readonly OidcSettings _options;

    /// <summary>
    /// Initializes a new <see cref="Auth0OidcUserInfoClient"/> using the provided
    /// <see cref="HttpClient"/> and OIDC configuration monitor.
    /// </summary>
    /// <param name="http">The HTTP client used to call Auth0’s UserInfo endpoint.</param>
    /// <param name="options">
    /// Provides dynamically reloadable OIDC configuration values.
    /// </param>
    public Auth0OidcUserInfoClient(HttpClient http, IOptionsMonitor<OidcSettings> options)
    {
        _http = http;
        _options = options.CurrentValue;
    }

    /// <summary>
    /// Retrieves user profile information from Auth0’s <c>/userinfo</c> endpoint
    /// using the provided access token.
    /// <para>
    /// The method performs:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Bearer‑authenticated GET request</description></item>
    /// <item><description>JSON parsing of standard OIDC claims</description></item>
    /// <item><description>Extraction of all string‑typed claims</description></item>
    /// </list>
    /// <para>
    /// If the request fails or the response is malformed, an
    /// <see cref="OidcProtocolException"/> is thrown.
    /// </para>
    /// </summary>
    /// <param name="accessToken">The access token used to authenticate the request.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>
    /// A populated <see cref="OidcUserInfo"/> instance containing standard OIDC
    /// profile claims and all additional string‑based claims returned by Auth0.
    /// </returns>
    /// <exception cref="OidcProtocolException">
    /// Thrown when the UserInfo endpoint returns a non‑success status code.
    /// </exception>
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

    /// <summary>
    /// Extracts all string‑typed claims from the UserInfo JSON payload.
    /// <para>
    /// This includes both standard OIDC profile claims and any additional
    /// provider‑specific claims returned by Auth0.
    /// </para>
    /// </summary>
    /// <param name="root">The root JSON element returned by the UserInfo endpoint.</param>
    /// <returns>
    /// A dictionary mapping claim names to their string values.
    /// </returns>
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
