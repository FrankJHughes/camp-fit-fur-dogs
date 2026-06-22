using Frank.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContextBuilder;
using Frank.ImmutableContextBuilder;

namespace Frank.Authentication.Callback.Oidc;

public sealed class OidcAuthCallbackContextBuilder
    : ImmutableContextBuilderBase<OidcAuthCallbackContext, IImmutableContextBuildStep<OidcAuthCallbackContext>>,
      IImmutableContextBuilder<FrankAuthCallbackRequest, OidcAuthCallbackContext, FrankAuthCallbackResult>
{
    public OidcAuthCallbackContextBuilder(
        IEnumerable<IImmutableContextBuildStep<OidcAuthCallbackContext>> steps,
        Action<ImmutableContextBuilderDiagnosticEvent>? trace = null)
        : base(steps, trace)
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
        Trace(new ImmutableContextBuilderDiagnosticEvent(
            StepId: step.Metadata.Id,
            StepName: step.Metadata.DisplayName,
            Phase: "Start",
            DurationMs: null,
            Before: before,
            After: before));
    }

    protected override void EmitEndEvent(
        IImmutableContextBuildStep<OidcAuthCallbackContext> step,
        OidcAuthCallbackContext before,
        OidcAuthCallbackContext after,
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
