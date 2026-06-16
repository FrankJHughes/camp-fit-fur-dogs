namespace Frank.Authentication.Callback.Oidc;

public interface IOidcUserInfoClient
{
    Task<OidcUserInfo> GetUserInfoAsync(string accessToken, CancellationToken ct);
}
