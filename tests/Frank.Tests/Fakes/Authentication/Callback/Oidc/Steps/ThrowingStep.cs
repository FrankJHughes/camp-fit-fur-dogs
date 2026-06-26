using Frank.Abstractions.ImmutableContextBuilder;
using Frank.Authentication.Callback.Oidc;

namespace Frank.Tests.Fakes.Authentication.Callback.Oidc.Steps;

public sealed class ThrowingStep : IImmutableContextBuildStep<OidcAuthCallbackContext>
{
    public bool CanExecute(OidcAuthCallbackContext ctx) => true;

    public Task<OidcAuthCallbackContext> ExecuteAsync(OidcAuthCallbackContext ctx, CancellationToken ct)
        => throw new InvalidOperationException("Boom");

    public IImmutableContextBuildStepMetadata Metadata => new ImmutableContextBuildStepMetadata("Throw", "Throw");
}
