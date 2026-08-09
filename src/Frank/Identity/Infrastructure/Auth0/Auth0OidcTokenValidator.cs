using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Frank.Identity.Infrastructure.Auth0;

/// <summary>
/// Auth0 implementation of <see cref="IOidcTokenValidator"/> responsible for
/// validating ID tokens returned by the OIDC authorization code flow.
/// <para>
/// This validator retrieves Auth0's JWKS, constructs appropriate token validation
/// parameters, and performs full JWT validation including issuer, audience,
/// signature, and lifetime checks.
/// </para>
/// <para>
/// The class uses <see cref="IOptionsMonitor{T}"/> to support dynamic configuration
/// reloads, ensuring that OIDC settings remain current without requiring
/// application restarts.
/// </para>
/// </summary>
public sealed class Auth0OidcTokenValidator : IOidcTokenValidator
{
    private readonly HttpClient _http;
    private readonly OidcSettings _options;

    /// <summary>
    /// Initializes a new <see cref="Auth0OidcTokenValidator"/> using the provided
    /// <see cref="HttpClient"/> and OIDC configuration monitor.
    /// </summary>
    /// <param name="http">The HTTP client used to retrieve JWKS from Auth0.</param>
    /// <param name="options">
    /// Provides dynamically reloadable OIDC configuration values.
    /// </param>
    public Auth0OidcTokenValidator(HttpClient http, IOptionsMonitor<OidcSettings> options)
    {
        _http = http;
        _options = options.CurrentValue;
    }

    /// <summary>
    /// Validates an Auth0-issued ID token using Auth0's JWKS and standard JWT
    /// validation rules.
    /// <para>
    /// The method performs:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Signature validation using Auth0's JWKS</description></item>
    /// <item><description>Issuer validation</description></item>
    /// <item><description>Audience validation</description></item>
    /// <item><description>Lifetime validation</description></item>
    /// <item><description>Extraction of the <c>sub</c> claim</description></item>
    /// </list>
    /// <para>
    /// If validation fails, an <see cref="OidcProtocolException"/> is thrown with a
    /// descriptive message.
    /// </para>
    /// </summary>
    /// <param name="idToken">The raw ID token returned by Auth0.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>
    /// An <see cref="OidcTokenValidationResult"/> containing the subject identifier
    /// and all string-based claims.
    /// </returns>
    /// <exception cref="OidcProtocolException">
    /// Thrown when validation fails or when required claims are missing.
    /// </exception>
    public async Task<OidcTokenValidationResult> ValidateAsync(string idToken, CancellationToken ct)
    {
        var handler = new JwtSecurityTokenHandler();
        var validationParameters = await BuildValidationParametersAsync(ct);

        ClaimsPrincipal principal;
        SecurityToken validatedToken;

        try
        {
            principal = handler.ValidateToken(idToken, validationParameters, out validatedToken);
        }
        catch (Exception ex)
        {
            throw new OidcProtocolException($"ID token validation failed: {ex.Message}");
        }

        var jwt = (JwtSecurityToken)validatedToken;

        var subject = principal.FindFirstValue("sub")
            ?? jwt.Subject
            ?? throw new OidcProtocolException("ID token missing 'sub' claim.");

        var claims = jwt.Claims
            .Where(c => c.ValueType == ClaimValueTypes.String)
            .ToDictionary(c => c.Type, c => c.Value);

        return new OidcTokenValidationResult(subject, claims);
    }

    /// <summary>
    /// Builds the <see cref="TokenValidationParameters"/> required to validate
    /// Auth0-issued ID tokens.
    /// <para>
    /// This includes retrieving Auth0's JWKS, configuring issuer and audience
    /// validation, enabling signature and lifetime checks, and applying a small
    /// clock skew tolerance.
    /// </para>
    /// </summary>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>
    /// A fully configured <see cref="TokenValidationParameters"/> instance.
    /// </returns>
    /// <exception cref="OidcProtocolException">
    /// Thrown when JWKS retrieval fails.
    /// </exception>
    private async Task<TokenValidationParameters> BuildValidationParametersAsync(CancellationToken ct)
    {
        var jwksUri = $"{_options.Authority}/.well-known/jwks.json";

        var response = await _http.GetAsync(jwksUri, ct);
        if (!response.IsSuccessStatusCode)
            throw new OidcProtocolException("Failed to retrieve JWKS from Auth0.");

        var json = await response.Content.ReadAsStringAsync(ct);
        var jwks = new JsonWebKeySet(json);

        return new TokenValidationParameters
        {
            ValidIssuer = $"{_options.Authority}/",
            ValidAudience = _options.ClientId,
            IssuerSigningKeys = jwks.Keys,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    }
}
