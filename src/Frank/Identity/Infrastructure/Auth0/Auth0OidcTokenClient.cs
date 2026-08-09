using System.Text.Json;
using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Frank.Identity.Infrastructure.Auth0;

/// <summary>
/// Auth0 implementation of <see cref="IOidcTokenClient"/> responsible for
/// exchanging an authorization code for OIDC tokens.
/// <para>
/// This client communicates directly with Auth0’s <c>/oauth/token</c> endpoint
/// using <see cref="HttpClient"/> and structured form‑encoded requests.
/// </para>
/// <para>
/// The class uses <see cref="IOptionsMonitor{T}"/> to support dynamic configuration
/// reloads, ensuring that OIDC settings (authority, client credentials, callback
/// URL) remain up‑to‑date without requiring application restarts.
/// </para>
/// </summary>
public sealed class Auth0OidcTokenClient : IOidcTokenClient
{
    private readonly HttpClient _http;
    private readonly OidcSettings _options;

    /// <summary>
    /// Initializes a new <see cref="Auth0OidcTokenClient"/> using the provided
    /// <see cref="HttpClient"/> and OIDC configuration.
    /// </summary>
    /// <param name="http">The HTTP client used to call Auth0’s token endpoint.</param>
    /// <param name="optionsMonitor">
    /// Provides dynamically reloadable OIDC configuration values.
    /// </param>
    public Auth0OidcTokenClient(HttpClient http, IOptionsMonitor<OidcSettings> optionsMonitor)
    {
        _http = http;
        _options = optionsMonitor.CurrentValue;
    }

    /// <summary>
    /// Exchanges an authorization code for OIDC tokens using Auth0’s
    /// <c>/oauth/token</c> endpoint.
    /// <para>
    /// The method sends a form‑encoded POST request containing:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>grant_type = authorization_code</c></description></item>
    /// <item><description>Client credentials</description></item>
    /// <item><description>The received authorization code</description></item>
    /// <item><description>The configured redirect URI</description></item>
    /// </list>
    /// <para>
    /// The response is parsed for <c>access_token</c> and <c>id_token</c>.
    /// An <see cref="OidcProtocolException"/> is thrown if the exchange fails or
    /// if the access token is missing.
    /// </para>
    /// </summary>
    /// <param name="authorizationCode">The authorization code received from Auth0.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>
    /// An <see cref="OidcTokenResponse"/> containing the access token and optional ID token.
    /// </returns>
    /// <exception cref="OidcProtocolException">
    /// Thrown when the token endpoint returns a non‑success status code or when
    /// the access token is missing from the response.
    /// </exception>
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
