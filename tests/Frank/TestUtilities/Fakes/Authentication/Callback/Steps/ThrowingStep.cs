using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;

namespace Frank.Core.Application.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class ThrowingStep : IImmutableContextBuildStep<CallbackSaveContext>
{
    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("Throw", "Throwing Step");

    public bool CanExecute(CallbackSaveContext ctx) => true;

    public Task<CallbackSaveContext> ExecuteAsync(
        CallbackSaveContext ctx,
        CancellationToken ct)
        => throw new InvalidOperationException("Test exception");
}
