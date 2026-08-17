using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;

namespace Frank.Core.Application.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class NoOpStep : IImmutableContextBuildStep<CallbackSaveContext>
{
    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("NoOp", "No‑Op Step");

    public bool CanExecute(CallbackSaveContext ctx) => true;

    public Task<CallbackSaveContext> ExecuteAsync(
        CallbackSaveContext ctx,
        CancellationToken ct)
        => Task.FromResult(ctx);
}
