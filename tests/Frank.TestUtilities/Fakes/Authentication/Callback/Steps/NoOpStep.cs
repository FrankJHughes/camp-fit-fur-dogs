using System.Threading;
using System.Threading.Tasks;
using Frank.Abstractions.ImmutableContext;
using Frank.Application.Abstractions.Identity.Callback;

namespace Frank.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class NoOpStep : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
{
    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("NoOp", "No‑Op Step");

    public bool CanExecute(ApplicationAuthCallbackContext ctx) => true;

    public Task<ApplicationAuthCallbackContext> ExecuteAsync(
        ApplicationAuthCallbackContext ctx,
        CancellationToken ct)
        => Task.FromResult(ctx);
}
