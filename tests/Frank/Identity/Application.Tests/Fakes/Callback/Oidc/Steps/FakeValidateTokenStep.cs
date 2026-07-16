using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Tests.Fakes.Callback.Oidc.Steps;

public sealed class FakeValidateTokensStep : IImmutableContextBuildStep<OidcCallbackContext>
{
    private readonly bool _shouldThrow;

    public FakeValidateTokensStep(bool shouldThrow = false)
    {
        _shouldThrow = shouldThrow;
    }

    public bool CanExecute(OidcCallbackContext ctx) => true;

    public Task<OidcCallbackContext> ExecuteAsync(
        OidcCallbackContext ctx,
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
