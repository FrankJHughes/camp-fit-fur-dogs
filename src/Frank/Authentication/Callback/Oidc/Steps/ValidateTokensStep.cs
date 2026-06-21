using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Frank.Abstractions.ImmutableContext;
using Frank.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Frank.Authentication.Callback.Oidc.Steps;

public sealed class ValidateTokensStep : IImmutableContextBuildStep<OidcAuthCallbackContext>
{
    private readonly AuthCallbackOidcSettings _options;
    private readonly HttpClient _http;

    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.validate-tokens", "Validate ID Token");

    public ValidateTokensStep(AuthCallbackOidcSettings options, HttpClient http)
    {
        _options = options;
        _http = http;
    }

    public bool CanExecute(OidcAuthCallbackContext ctx)
        => ctx.IdToken is not null;

    public async Task<OidcAuthCallbackContext> ExecuteAsync(
        OidcAuthCallbackContext ctx,
        CancellationToken ct)
    {
        var idToken = ctx.IdToken!;
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

        return ctx with
        {
            SubjectId = subject,
            Claims = claims
        };
    }

    private async Task<TokenValidationParameters> BuildValidationParametersAsync(CancellationToken ct)
    {
        var jwksUri = $"https://{_options.Authority}/.well-known/jwks.json";

        var response = await _http.GetAsync(jwksUri, ct);
        if (!response.IsSuccessStatusCode)
            throw new OidcProtocolException("Failed to retrieve JWKS from Auth0.");

        var json = await response.Content.ReadAsStringAsync(ct);
        var jwks = new JsonWebKeySet(json);

        return new TokenValidationParameters
        {
            ValidIssuer = $"https://{_options.Authority}/",
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
