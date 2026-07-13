using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Frank.Identity.Application.Callback.Oidc.Steps;

public sealed class ValidateTokensStep : IImmutableContextBuildStep<OidcCallbackContext>
{
    private readonly OidcCallbackSettings _options;
    private readonly HttpClient _http;

    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.validate-tokens", "Validate ID Token");

    public ValidateTokensStep(IOptionsMonitor<OidcCallbackSettings> options, HttpClient http)
    {
        _options = options.CurrentValue;
        _http = http;
    }

    public bool CanExecute(OidcCallbackContext ctx)
        => ctx.IdToken is not null;

    public async Task<OidcCallbackContext> ExecuteAsync(
        OidcCallbackContext ctx,
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
