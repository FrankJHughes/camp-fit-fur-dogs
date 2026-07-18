namespace Frank.Identity.Application.Abstractions.Oidc;

public interface IOidcUserInfoClient
{
    Task<OidcUserInfo> GetUserInfoAsync(string accessToken, CancellationToken ct);
}
