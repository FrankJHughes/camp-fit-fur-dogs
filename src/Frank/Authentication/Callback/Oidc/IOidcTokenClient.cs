namespace Frank.Authentication.Callback.Oidc;

public interface IOidcTokenClient
{
    Task<string> ExchangeCodeAsync(string code, CancellationToken ct);
}
