using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Frank.Identity.Infrastructure.Auth0;

public sealed class Auth0OidcTokenValidator : IOidcTokenValidator
{
    private readonly HttpClient _http;
    private readonly OidcCallbackSettings _options;

    public Auth0OidcTokenValidator(HttpClient http, IOptionsMonitor<OidcCallbackSettings> options)
    {
        _http = http;
        _options = options.CurrentValue;
    }

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
