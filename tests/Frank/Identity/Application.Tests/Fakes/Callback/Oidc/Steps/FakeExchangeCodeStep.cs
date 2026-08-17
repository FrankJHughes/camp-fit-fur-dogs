using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Tests.Fakes.Callback.Oidc.Steps;

public sealed class FakeExchangeCodeStep : IImmutableContextBuildStep<CallbackOidcContext>
{
    private readonly string _accessToken;
    private readonly string? _idToken;

    public FakeExchangeCodeStep(string accessToken, string? idToken = null)
    {
        _accessToken = accessToken;
        _idToken = idToken;
    }

    public bool CanExecute(CallbackOidcContext ctx) => true;

    public Task<CallbackOidcContext> ExecuteAsync(
        CallbackOidcContext ctx,
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
