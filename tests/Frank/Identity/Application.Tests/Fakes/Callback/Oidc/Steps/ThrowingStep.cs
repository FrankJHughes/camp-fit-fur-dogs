using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Tests.Fakes.Callback.Oidc.Steps;

public sealed class ThrowingStep : IImmutableContextBuildStep<CallbackOidcContext>
{
    public bool CanExecute(CallbackOidcContext ctx) => true;

    public Task<CallbackOidcContext> ExecuteAsync(CallbackOidcContext ctx, CancellationToken ct)
        => throw new InvalidOperationException("Boom");

    public IImmutableContextBuildStepMetadata Metadata => new ImmutableContextBuildStepMetadata("Throw", "Throw");
}
