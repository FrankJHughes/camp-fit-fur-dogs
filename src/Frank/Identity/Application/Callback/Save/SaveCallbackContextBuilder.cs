using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Core.Application.Abstractions.Observations;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Core.Application.ImmutableContext;

namespace Frank.Identity.Application.Callback.Save;

public sealed class SaveCallbackContextBuilder
    : ImmutableContextBuilderBase<SaveCallbackContext, IImmutableContextBuildStep<SaveCallbackContext>>,
      IImmutableContextBuilder<SaveCallbackContextBuilderRequest, SaveCallbackContext, SaveCallbackContextBuilderResult>
{
    public SaveCallbackContextBuilder(
        IEnumerable<IImmutableContextBuildStep<SaveCallbackContext>> steps,
        IObservationSink sink,
        Func<string, string, IObservationContext> contextFactory)
        : base(steps, sink, contextFactory("System", "SaveCallbackContextBuilder"))
    {
    }

    public async Task<SaveCallbackContextBuilderResult> BuildAsync(
        SaveCallbackContextBuilderRequest request,
        CancellationToken ct)
    {
        var ctx = new SaveCallbackContext
        {
            External = request.External,
            Now = request.Now
        };

        ctx = await ProcessAsync(ctx, ct);

        return new SaveCallbackContextBuilderResult
        {
            UserId = ctx.UserId!.Value,
            SessionId = ctx.SessionId!.Value,
            TokenHash = ctx.TokenHash!,
            CookieValue = ctx.CookieValue!
        };
    }

    protected override void AssertValidTransition(
        IImmutableContextBuildStep<SaveCallbackContext> step,
        SaveCallbackContext before,
        SaveCallbackContext after)
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
        IImmutableContextBuildStep<SaveCallbackContext> step,
        SaveCallbackContext before)
    {
        Sink.Emit(
            eventName: "SaveCallback.StepStart",
            category: "SaveCallback",
            severity: "Info",
            payload: new
            {
                StepId = step.Metadata.Id,
                StepName = step.Metadata.DisplayName,
                StepType = step.GetType().FullName,
                BeforeType = before.GetType().FullName
            },
            context: SystemContext);
    }

    protected override void EmitEndEvent(
        IImmutableContextBuildStep<SaveCallbackContext> step,
        SaveCallbackContext before,
        SaveCallbackContext? after,
        long durationMs)
    {
        Sink.Emit(
            eventName: "SaveCallback.StepEnd",
            category: "SaveCallback",
            severity: "Info",
            payload: new
            {
                StepId = step.Metadata.Id,
                StepName = step.Metadata.DisplayName,
                StepType = step.GetType().FullName,
                BeforeType = before.GetType().FullName,
                AfterType = after?.GetType().FullName,
                DurationMs = durationMs
            },
            context: SystemContext);
    }
}
