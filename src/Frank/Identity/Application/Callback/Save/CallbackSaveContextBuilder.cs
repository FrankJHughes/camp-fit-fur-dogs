using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Core.Application.Abstractions.Observations;
using Frank.Core.Application.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;

namespace Frank.Identity.Application.Callback.Save;

/// <summary>
/// Builds a fully enriched <see cref="CallbackSaveContext"/> by executing the
/// ordered set of immutable Save‑pipeline steps.
/// <para>
/// The Save pipeline runs after the OIDC callback pipeline has produced a
/// validated external identity context. Its responsibilities include:
/// resolving the internal user, generating a session token and cookie,
/// creating the authenticated session, and emitting audit logs.
/// </para>
/// <para>
/// This builder enforces immutability guarantees, emits structured observability
/// events, and ensures that each step performs a valid and safe transformation.
/// </para>
/// </summary>
public sealed class CallbackSaveContextBuilder :
    ImmutableContextBuilderBase<CallbackSaveContext, IImmutableContextBuildStep<CallbackSaveContext>>,
    ICallbackSaveContextBuilder
{
    /// <summary>
    /// Creates a new <see cref="CallbackSaveContextBuilder"/> using the provided
    /// pipeline steps, observation sink, and observation context factory.
    /// </summary>
    /// <param name="steps">
    /// The ordered set of immutable Save‑pipeline steps to execute.
    /// </param>
    /// <param name="sink">
    /// The observation sink used to emit structured diagnostics for each step.
    /// </param>
    /// <param name="contextFactory">
    /// A factory that produces an <see cref="IObservationContext"/> for pipeline
    /// events. The builder uses the context <c>("System", "CallbackSaveContextBuilder")</c>.
    /// </param>
    public CallbackSaveContextBuilder(
        IEnumerable<IImmutableContextBuildStep<CallbackSaveContext>> steps,
        IObservationSink sink,
        Func<string, string, IObservationContext> contextFactory)
        : base(steps, sink, contextFactory("System", "CallbackSaveContextBuilder"))
    {
    }

    /// <summary>
    /// Executes the Save pipeline and produces a fully enriched
    /// <see cref="CallbackSaveContextBuilderResult"/>.
    /// <para>
    /// The pipeline begins with a minimal context containing the external
    /// identity information and the current timestamp, then executes each step
    /// in order. After execution, the builder returns a result containing the
    /// resolved user ID, created session ID, token hash, and cookie value.
    /// </para>
    /// </summary>
    /// <param name="request">
    /// The request containing external identity information and the timestamp
    /// at which the Save pipeline begins.
    /// </param>
    /// <param name="ct">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="CallbackSaveContextBuilderResult"/> containing all values
    /// produced by the Save pipeline.
    /// </returns>
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

    /// <summary>
    /// Ensures that the pipeline step performed a valid immutable transformation.
    /// <para>
    /// This method enforces immutability guarantees by verifying that steps do
    /// not modify fields that must remain constant throughout the pipeline,
    /// such as <c>External</c> and <c>Now</c>.
    /// </para>
    /// </summary>
    /// <param name="step">The step being validated.</param>
    /// <param name="before">The context before the step executed.</param>
    /// <param name="after">The context returned by the step.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the step returns <c>null</c> or modifies immutable fields.
    /// </exception>
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

    /// <summary>
    /// Emits a structured observability event indicating that a pipeline step is
    /// about to begin execution.
    /// </summary>
    /// <param name="step">The step that is starting.</param>
    /// <param name="before">The context before the step executes.</param>
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

    /// <summary>
    /// Emits a structured observability event indicating that a pipeline step has
    /// completed execution.
    /// </summary>
    /// <param name="step">The step that finished executing.</param>
    /// <param name="before">The context before the step executed.</param>
    /// <param name="after">The context returned by the step.</param>
    /// <param name="durationMs">The duration of the step execution in milliseconds.</param>
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
