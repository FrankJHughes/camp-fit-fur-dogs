namespace Frank.Identity.Application.Abstractions.Oidc;

public interface IOidcTokenClient
{
    Task<OidcTokenResponse> ExchangeCodeAsync(string code, CancellationToken ct);
}

public sealed record OidcTokenResponse(
    string AccessToken,
    string? IdToken);
