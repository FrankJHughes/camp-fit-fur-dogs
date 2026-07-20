namespace Frank.Identity.Application.Abstractions.Oidc;

public interface IOidcTokenValidator
{
    Task<OidcTokenValidationResult> ValidateAsync(string idToken, CancellationToken ct);
}

public sealed record OidcTokenValidationResult(
    string SubjectId,
    IReadOnlyDictionary<string, string> Claims);
