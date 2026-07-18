using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Core.Application.Abstractions.Observations;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Core.Application.ImmutableContexts;

namespace Frank.Identity.Application.Callback.Save;

public sealed class CallbackSaveContextBuilder :
    ImmutableContextBuilderBase<CallbackSaveContext, IImmutableContextBuildStep<CallbackSaveContext>>,
    ICallbackSaveContextBuilder
{
    public CallbackSaveContextBuilder(
        IEnumerable<IImmutableContextBuildStep<CallbackSaveContext>> steps,
        IObservationSink sink,
        Func<string, string, IObservationContext> contextFactory)
        : base(steps, sink, contextFactory("System", "CallbackSaveContextBuilder"))
    {
    }

    public async Task<CallbackSaveContextBuilderResult> BuildAsync(
        CallbackSaveContextBuilderRequest request,
        CancellationToken ct)
    {
        var ctx = new CallbackSaveContext
        {
            External = request.External,
            Now = request.Now
        };

        ctx = await ProcessAsync(ctx, ct);

        return new CallbackSaveContextBuilderResult
        {
            UserId = ctx.UserId!.Value,
            SessionId = ctx.SessionId!.Value,
            TokenHash = ctx.TokenHash!,
            CookieValue = ctx.CookieValue!
        };
    }

    protected override void AssertValidTransition(
        IImmutableContextBuildStep<CallbackSaveContext> step,
        CallbackSaveContext before,
        CallbackSaveContext after)
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
        IImmutableContextBuildStep<CallbackSaveContext> step,
        CallbackSaveContext before)
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
        IImmutableContextBuildStep<CallbackSaveContext> step,
        CallbackSaveContext before,
        CallbackSaveContext? after,
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
