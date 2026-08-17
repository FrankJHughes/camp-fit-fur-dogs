using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Core.Application.Abstractions.Observations;
using Frank.Core.Application.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Callback.Oidc;

/// <summary>
/// Builds a fully enriched <see cref="CallbackOidcContext"/> by executing a
/// sequence of immutable OIDC pipeline steps.
/// <para>
/// This builder orchestrates the entire OIDC authentication callback pipeline:
/// exchanging the authorization code, validating tokens, fetching UserInfo,
/// extracting claims, and producing a final <see cref="CallbackOidcContextBuilderResult"/>.
/// </para>
/// <para>
/// The builder enforces immutability guarantees, emits structured observability
/// events, and ensures that each step performs a valid and safe transformation.
/// </para>
/// </summary>
public sealed class CallbackOidcContextBuilder
    : ImmutableContextBuilderBase<CallbackOidcContext, IImmutableContextBuildStep<CallbackOidcContext>>,
      ICallbackOidcContextBuilder
{
    /// <summary>
    /// Creates a new <see cref="CallbackOidcContextBuilder"/> using the provided
    /// pipeline steps, observation sink, and observation context factory.
    /// </summary>
    /// <param name="steps">
    /// The ordered set of immutable OIDC pipeline steps to execute.
    /// </param>
    /// <param name="sink">
    /// The observation sink used to emit structured diagnostics for each step.
    /// </param>
    /// <param name="contextFactory">
    /// A factory that produces an <see cref="IObservationContext"/> for pipeline
    /// events. The builder uses the context <c>("System", "OidcAuthCallbackContextBuilder")</c>.
    /// </param>
    public CallbackOidcContextBuilder(
        IEnumerable<IImmutableContextBuildStep<CallbackOidcContext>> steps,
        IObservationSink sink,
        Func<string, string, IObservationContext> contextFactory)
        : base(steps, sink, contextFactory("System", "OidcAuthCallbackContextBuilder"))
    {
    }

    /// <summary>
    /// Executes the OIDC callback pipeline and produces a fully enriched
    /// <see cref="CallbackOidcContextBuilderResult"/>.
    /// <para>
    /// The pipeline begins with a minimal context containing the authorization
    /// code and timestamp, then executes each step in order. After execution,
    /// the builder ensures that a valid <c>SubjectId</c> is present.
    /// </para>
    /// </summary>
    /// <param name="request">
    /// The request containing the authorization code and any initial callback data.
    /// </param>
    /// <param name="ct">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="CallbackOidcContextBuilderResult"/> containing validated claims,
    /// profile information, and the resolved subject identifier.
    /// </returns>
    /// <exception cref="OidcProtocolException">
    /// Thrown when the pipeline completes without producing a <c>SubjectId</c>.
    /// </exception>
    public async Task<CallbackOidcContextBuilderResult> BuildAsync(
        CallbackOidcContextBuilderRequest request,
        CancellationToken ct)
    {
        var ctx = new CallbackOidcContext
        {
            Code = request.Code,
            Timestamp = DateTimeOffset.UtcNow
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

    /// <summary>
    /// Ensures that the pipeline step performed a valid immutable transformation.
    /// <para>
    /// This method enforces immutability guarantees by verifying that steps do not
    /// modify fields that must remain constant throughout the pipeline, such as
    /// <c>Code</c> and <c>Timestamp</c>.
    /// </para>
    /// </summary>
    /// <param name="step">The step being validated.</param>
    /// <param name="before">The context before the step executed.</param>
    /// <param name="after">The context returned by the step.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the step returns <c>null</c> or modifies immutable fields.
    /// </exception>
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

        if (after.Timestamp != before.Timestamp)
            throw new InvalidOperationException(
                $"Step '{step.Metadata.Id}' modified immutable field 'Now'.");
    }

    /// <summary>
    /// Emits a structured observability event indicating that a pipeline step is
    /// about to begin execution.
    /// </summary>
    /// <param name="step">The step that is starting.</param>
    /// <param name="before">The context before the step executes.</param>
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

    /// <summary>
    /// Emits a structured observability event indicating that a pipeline step has
    /// completed execution.
    /// </summary>
    /// <param name="step">The step that finished executing.</param>
    /// <param name="before">The context before the step executed.</param>
    /// <param name="after">The context returned by the step.</param>
    /// <param name="durationMs">The duration of the step execution in milliseconds.</param>
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
