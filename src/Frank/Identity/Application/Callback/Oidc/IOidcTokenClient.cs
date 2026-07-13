namespace Frank.Identity.Application.Callback.Oidc;

public interface IOidcTokenClient
{
    Task<string> ExchangeCodeAsync(string code, CancellationToken ct);
}
