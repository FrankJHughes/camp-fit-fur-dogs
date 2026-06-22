using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContextBuilder;

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
