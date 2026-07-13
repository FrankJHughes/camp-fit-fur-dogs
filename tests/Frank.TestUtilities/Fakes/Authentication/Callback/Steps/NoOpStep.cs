using System.Threading;
using System.Threading.Tasks;
using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Identity.Application.Abstractions.Callback.Save;

namespace Frank.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class NoOpStep : IImmutableContextBuildStep<SaveCallbackContext>
{
    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("NoOp", "No‑Op Step");

    public bool CanExecute(SaveCallbackContext ctx) => true;

    public Task<SaveCallbackContext> ExecuteAsync(
        SaveCallbackContext ctx,
        CancellationToken ct)
        => Task.FromResult(ctx);
}
