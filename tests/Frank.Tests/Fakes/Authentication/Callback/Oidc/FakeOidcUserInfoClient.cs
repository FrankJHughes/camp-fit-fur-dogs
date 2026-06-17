using Frank.Authentication.Callback.Oidc;

namespace Frank.Tests.Fakes.Authentication.Callback.Oidc;

public sealed class FakeOidcUserInfoClient : IOidcUserInfoClient
{
    public OidcUserInfo? Response { get; set; }
    public bool ShouldThrow { get; set; }

    public Task<OidcUserInfo> GetUserInfoAsync(string accessToken, CancellationToken ct)
    {
        if (ShouldThrow)
            throw new InvalidOperationException("Fake userinfo failure");

        if (Response is null)
            throw new InvalidOperationException("No fake userinfo response configured");

        return Task.FromResult(Response);
    }
}
