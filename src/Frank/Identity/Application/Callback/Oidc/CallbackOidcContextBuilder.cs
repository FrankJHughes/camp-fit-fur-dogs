using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Core.Application.Abstractions.Observations;
using Frank.Core.Application.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Callback.Oidc;

public sealed class CallbackOidcContextBuilder
    : ImmutableContextBuilderBase<CallbackOidcContext, IImmutableContextBuildStep<CallbackOidcContext>>,
      ICallbackOidcContextBuilder
{
    public CallbackOidcContextBuilder(
        IEnumerable<IImmutableContextBuildStep<CallbackOidcContext>> steps,
        IObservationSink sink,
        Func<string, string, IObservationContext> contextFactory)
        : base(steps, sink, contextFactory("System", "OidcAuthCallbackContextBuilder"))
    {
    }

    public async Task<CallbackOidcContextBuilderResult> BuildAsync(
        CallbackOidcContextBuilderRequest request,
        CancellationToken ct)
    {
        var ctx = new CallbackOidcContext
        {
            Code = request.Code,
            Now = DateTimeOffset.UtcNow
        };

        ctx = await ProcessAsync(ctx, ct);

        if (ctx.SubjectId is null)
            throw new OidcProtocolException("OIDC pipeline completed without a SubjectId.");

        return new CallbackOidcContextBuilderResult
        {
            SubjectId = ctx.SubjectId,
            Claims = ctx.Claims ?? new Dictionary<string, string>(),
            Email = ctx.Email,
            GivenName = ctx.GivenName,
            FamilyName = ctx.FamilyName,
            Picture = ctx.Picture,
            Provider = ctx.Provider
        };
    }

    protected override void AssertValidTransition(
        IImmutableContextBuildStep<CallbackOidcContext> step,
        CallbackOidcContext before,
        CallbackOidcContext after)
    {
        if (after is null)
            throw new InvalidOperationException(
                $"Pipeline step '{step.Metadata.Id}' returned null context.");

        if (after.Code != before.Code)
            throw new InvalidOperationException(
                $"Step '{step.Metadata.Id}' modified immutable field 'Code'.");

        if (after.Now != before.Now)
            throw new InvalidOperationException(
                $"Step '{step.Metadata.Id}' modified immutable field 'Now'.");
    }

    protected override void EmitStartEvent(
        IImmutableContextBuildStep<CallbackOidcContext> step,
        CallbackOidcContext before)
    {
        Sink.Emit(
            eventName: "OidcAuthCallback.StepStart",
            category: "OidcAuthCallback",
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
        IImmutableContextBuildStep<CallbackOidcContext> step,
        CallbackOidcContext before,
        CallbackOidcContext after,
        long durationMs)
    {
        Sink.Emit(
            eventName: "OidcAuthCallback.StepEnd",
            category: "OidcAuthCallback",
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
