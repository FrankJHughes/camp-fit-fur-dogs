using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContext;
using Frank.ImmutableContext;

namespace CampFitFurDogs.Application.Authentication.Callback;

public sealed class ApplicationAuthCallbackContextBuilder
    : ImmutableContextBuilderBase<ApplicationAuthCallbackContext, IImmutableContextBuildStep<ApplicationAuthCallbackContext>>,
      IImmutableContextBuilder<ApplicationAuthCallbackRequest, ApplicationAuthCallbackContext, ApplicationAuthCallbackContextBuilderResult>
{
    public ApplicationAuthCallbackContextBuilder(
        IEnumerable<IImmutableContextBuildStep<ApplicationAuthCallbackContext>> steps,
        Action<ImmutableContextBuilderDiagnosticEvent>? trace = null)
        : base(steps, trace)
    {
    }

    public async Task<ApplicationAuthCallbackContextBuilderResult> BuildAsync(
        ApplicationAuthCallbackRequest request,
        CancellationToken ct)
    {
        var ctx = new ApplicationAuthCallbackContext
        {
            External = request.External,
            Now = request.Now,
            RequestedRedirectUrl = request.RequestedRedirectUrl
        };

        ctx = await ProcessAsync(ctx, ct);

        return new ApplicationAuthCallbackContextBuilderResult
        {
            CustomerId = ctx.CustomerId!.Value,
            SessionId = ctx.SessionId!.Value,
            TokenHash = ctx.TokenHash!,
            CookieValue = ctx.CookieValue!,
            RedirectUrl = ctx.RedirectUrl!
        };
    }

    protected override void AssertValidTransition(
        IImmutableContextBuildStep<ApplicationAuthCallbackContext> step,
        ApplicationAuthCallbackContext before,
        ApplicationAuthCallbackContext after)
    {
        if (after is null)
            throw new InvalidOperationException(
                $"Pipeline step '{step.Metadata.Id}' returned null context.");

        if (!ReferenceEquals(before.External, after.External))
            throw new InvalidOperationException(
                $"Step '{step.Metadata.Id}' modified immutable field 'External'.");

        if (after.Now != before.Now)
            throw new InvalidOperationException(
                $"Step '{step.Metadata.Id}' modified immutable field 'Now'.");
    }

    protected override void EmitStartEvent(
        IImmutableContextBuildStep<ApplicationAuthCallbackContext> step,
        ApplicationAuthCallbackContext before)
    {
        Trace(new ImmutableContextBuilderDiagnosticEvent(
            StepId: step.Metadata.Id,
            StepName: step.Metadata.DisplayName,
            Phase: "Start",
            DurationMs: null,
            Before: before,
            After: before));
    }

    protected override void EmitEndEvent(
        IImmutableContextBuildStep<ApplicationAuthCallbackContext> step,
        ApplicationAuthCallbackContext before,
        ApplicationAuthCallbackContext after,
        long durationMs)
    {
        Trace(new ImmutableContextBuilderDiagnosticEvent(
            StepId: step.Metadata.Id,
            StepName: step.Metadata.DisplayName,
            Phase: "End",
            DurationMs: durationMs,
            Before: before,
            After: after));
    }
}
