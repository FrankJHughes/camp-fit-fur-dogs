using Frank.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContext;
using Frank.Abstractions.Observability;
using Frank.ImmutableContext;

namespace Frank.Authentication.Callback.Oidc;

public sealed class OidcAuthCallbackContextBuilder
    : ImmutableContextBuilderBase<OidcAuthCallbackContext, IImmutableContextBuildStep<OidcAuthCallbackContext>>,
      IImmutableContextBuilder<FrankAuthCallbackRequest, OidcAuthCallbackContext, FrankAuthCallbackResult>
{
    public OidcAuthCallbackContextBuilder(
        IEnumerable<IImmutableContextBuildStep<OidcAuthCallbackContext>> steps,
        IObservabilitySink sink,
        IObservabilityContext systemContext)
        : base(steps, sink, systemContext)
    {
    }

    public async Task<FrankAuthCallbackResult> BuildAsync(
        FrankAuthCallbackRequest request,
        CancellationToken ct)
    {
        var ctx = new OidcAuthCallbackContext
        {
            Code = request.Code,
            Now = DateTimeOffset.UtcNow
        };

        ctx = await ProcessAsync(ctx, ct);

        if (ctx.SubjectId is null)
            throw new OidcProtocolException("OIDC pipeline completed without a SubjectId.");

        return new FrankAuthCallbackResult
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
        IImmutableContextBuildStep<OidcAuthCallbackContext> step,
        OidcAuthCallbackContext before,
        OidcAuthCallbackContext after)
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
        IImmutableContextBuildStep<OidcAuthCallbackContext> step,
        OidcAuthCallbackContext before)
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
        IImmutableContextBuildStep<OidcAuthCallbackContext> step,
        OidcAuthCallbackContext before,
        OidcAuthCallbackContext after,
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
