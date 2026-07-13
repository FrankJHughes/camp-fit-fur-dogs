namespace Frank.Identity.Application.Callback.Oidc;

public interface IOidcUserInfoClient
{
    Task<OidcUserInfo> GetUserInfoAsync(string accessToken, CancellationToken ct);
}
