namespace Frank.Identity.Application.Abstractions.Oidc;

public interface IOidcTokenClient
{
    Task<string> ExchangeCodeAsync(string code, CancellationToken ct);
}
