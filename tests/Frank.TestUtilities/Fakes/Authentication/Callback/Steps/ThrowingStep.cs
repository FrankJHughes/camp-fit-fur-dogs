using System;
using System.Threading;
using System.Threading.Tasks;
using Frank.Abstractions.ImmutableContext;
using Frank.Application.Abstractions.Identity.Callback;

namespace Frank.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class ThrowingStep : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
{
    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("Throw", "Throwing Step");

    public bool CanExecute(ApplicationAuthCallbackContext ctx) => true;

    public Task<ApplicationAuthCallbackContext> ExecuteAsync(
        ApplicationAuthCallbackContext ctx,
        CancellationToken ct)
        => throw new InvalidOperationException("Test exception");
}
