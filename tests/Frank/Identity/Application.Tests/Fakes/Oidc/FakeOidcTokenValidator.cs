using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Application.Callback.Oidc;

namespace Frank.Identity.Application.Tests.Fakes.Callback.Oidc;

public sealed class FakeOidcTokenValidator : IOidcTokenValidator
{
    public bool ThrowOnValidate { get; set; }
    public OidcTokenValidationResult? Result { get; set; }

    public Task<OidcTokenValidationResult> ValidateAsync(string idToken, CancellationToken ct)
    {
        if (ThrowOnValidate)
            throw new OidcProtocolException("Fake validator failure.");

        if (Result is null)
            throw new OidcProtocolException("No result configured.");

        return Task.FromResult(Result);
    }
}
