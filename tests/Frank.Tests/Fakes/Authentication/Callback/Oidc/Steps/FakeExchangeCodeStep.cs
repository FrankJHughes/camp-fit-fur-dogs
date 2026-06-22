using Frank.Abstractions.ImmutableContextBuilder;
using Frank.Authentication.Callback.Oidc;

namespace Frank.Tests.Fakes.Authentication.Callback.Oidc.Steps;

public sealed class FakeExchangeCodeStep : IImmutableContextBuildStep<OidcAuthCallbackContext>
{
    private readonly string _accessToken;
    private readonly string? _idToken;

    public FakeExchangeCodeStep(string accessToken, string? idToken = null)
    {
        _accessToken = accessToken;
        _idToken = idToken;
    }

    public bool CanExecute(OidcAuthCallbackContext ctx) => true;

    public Task<OidcAuthCallbackContext> ExecuteAsync(
        OidcAuthCallbackContext ctx,
        CancellationToken ct)
    {
        return Task.FromResult(ctx with
        {
            AccessToken = _accessToken,
            IdToken = _idToken
        });
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("FakeExchangeCode", "Fake Exchange Code");
}
