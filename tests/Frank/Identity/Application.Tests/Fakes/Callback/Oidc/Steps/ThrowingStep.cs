using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Tests.Fakes.Callback.Oidc.Steps;

public sealed class ThrowingStep : IImmutableContextBuildStep<OidcCallbackContext>
{
    public bool CanExecute(OidcCallbackContext ctx) => true;

    public Task<OidcCallbackContext> ExecuteAsync(OidcCallbackContext ctx, CancellationToken ct)
        => throw new InvalidOperationException("Boom");

    public IImmutableContextBuildStepMetadata Metadata => new ImmutableContextBuildStepMetadata("Throw", "Throw");
}
