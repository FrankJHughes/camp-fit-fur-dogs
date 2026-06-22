using Frank.Abstractions.ImmutableContextBuilder;
using Frank.Authentication.Callback.Oidc;

namespace Frank.Tests.Fakes.Authentication.Callback.Oidc.Steps;

public sealed class FakeValidateTokensStep : IImmutableContextBuildStep<OidcAuthCallbackContext>
{
    private readonly bool _shouldThrow;

    public FakeValidateTokensStep(bool shouldThrow = false)
    {
        _shouldThrow = shouldThrow;
    }

    public bool CanExecute(OidcAuthCallbackContext ctx) => true;

    public Task<OidcAuthCallbackContext> ExecuteAsync(
        OidcAuthCallbackContext ctx,
        CancellationToken cancellationToken)
    {
        if (_shouldThrow)
            throw new InvalidOperationException("Fake token validation failure");

        // In the real step, this validates:
        // - id_token signature
        // - nonce
        // - issuer
        // - audience
        // - expiration
        // - at_hash (optional)
        //
        // The fake simply returns the context unchanged.
        return Task.FromResult(ctx);
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("FakeValidateTokens", "Fake Validate Tokens");
}
