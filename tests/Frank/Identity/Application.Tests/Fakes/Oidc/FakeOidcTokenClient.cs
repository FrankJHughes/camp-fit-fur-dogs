using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Application.Callback.Oidc;

namespace Frank.Identity.Application.Tests.Fakes.Callback.Oidc;

public sealed class FakeOidcTokenClient : IOidcTokenClient
{
    public bool ThrowOnExchange { get; set; }
    public OidcTokenResponse? Result { get; set; }

    public Task<OidcTokenResponse> ExchangeCodeAsync(string authorizationCode, CancellationToken ct)
    {
        if (ThrowOnExchange)
            throw new OidcProtocolException("Fake token exchange failure.");

        if (Result is null)
            throw new OidcProtocolException("No fake result configured.");

        return Task.FromResult(Result);
    }
}
