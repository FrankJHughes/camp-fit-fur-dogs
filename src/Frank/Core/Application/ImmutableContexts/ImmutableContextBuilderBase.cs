using System.Diagnostics;
using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Core.Application.Abstractions.Observations;

namespace Frank.Core.Application.ImmutableContexts;

/// <summary>
/// Provides a base class for building immutable context objects through a
/// sequence of ordered build steps. Each step inspects the current context,
/// determines whether it can execute, and produces a new immutable context
/// instance.
///
/// <para>
/// The builder enforces immutability by requiring each step to return a new
/// <typeparamref name="TContext"/> instance. It also ensures that transitions
/// between states are validated through <see cref="AssertValidTransition"/>.
/// </para>
///
/// <para>
/// Build steps are executed in dependency‑aware order: each step declares
/// whether it can execute based on the current context, and the builder selects
/// the next eligible step until no steps remain. This allows flexible,
/// declarative pipelines without explicit ordering.
/// </para>
///
/// <para>
/// Observability hooks emit structured trace events at the start and end of
/// each step, including metadata, context types, and execution duration.
/// </para>
/// </summary>
/// <typeparam name="TContext">
/// The immutable context type being constructed.
/// </typeparam>
/// <typeparam name="TStep">
/// The step type responsible for transforming the context.
/// </typeparam>
public abstract class ImmutableContextBuilderBase<TContext, TStep>
    where TContext : ImmutableContextBase
    where TStep : IImmutableContextBuildStep<TContext>
{
    private readonly IReadOnlyList<TStep> _steps;

    /// <summary>
    /// Gets the observation sink used to emit structured trace events for
    /// step execution.
    /// </summary>
    protected IObservationSink Sink { get; }

    /// <summary>
    /// Gets the system‑level observation context used for all emitted events.
    /// </summary>
    protected IObservationContext SystemContext { get; }

    /// <summary>
    /// Initializes a new instance of the builder with the provided steps and
    /// observability components.
    /// </summary>
    /// <param name="steps">
    /// The ordered set of build steps that may execute during context
    /// construction.
    /// </param>
    /// <param name="sink">
    /// The observation sink used to emit trace events.
    /// </param>
    /// <param name="systemContext">
    /// The system‑level observation context associated with emitted events.
    /// </param>
    protected ImmutableContextBuilderBase(
        IEnumerable<TStep> steps,
        IObservationSink sink,
        IObservationContext systemContext)
    {
        _steps = steps.ToList();
        Sink = sink;
        SystemContext = systemContext;
    }

    /// <summary>
    /// Executes all eligible build steps in sequence, producing the final
    /// immutable context instance.
    /// </summary>
    /// <param name="ctx">
    /// The initial context instance.
    /// </param>
    /// <param name="ct">
    /// A cancellation token for the operation.
    /// </param>
    /// <returns>
    /// The fully constructed immutable context.
    /// </returns>
    protected async Task<TContext> ProcessAsync(TContext ctx, CancellationToken ct)
    {
        var remaining = new HashSet<TStep>(_steps);

        while (TrySelectNextStep(remaining, ctx, out var step))
        {
            var before = ctx;

            ctx = await ExecuteStepAsync(step!, before, ct);

            AssertValidTransition(step!, before, ctx);

            remaining.Remove(step!);
        }

        return ctx;
    }

    /// <summary>
    /// Validates that the transition from the <paramref name="before"/> context
    /// to the <paramref name="after"/> context is legal for the given step.
    /// </summary>
    /// <param name="step">The step that produced the new context.</param>
    /// <param name="before">The context prior to execution.</param>
    /// <param name="after">The context produced by the step.</param>
    protected abstract void AssertValidTransition(TStep step, TContext before, TContext after);

    // ------------------------------------------------------------
    // OBSERVABILITY HOOKS
    // ------------------------------------------------------------

    /// <summary>
    /// Emits a structured trace event indicating that a step is beginning
    /// execution.
    /// </summary>
    protected virtual void EmitStartEvent(TStep step, TContext before)
    {
        Sink.Emit(
            eventName: "ImmutableContextBuilder.StepStart",
            category: "ImmutableContextBuilder",
            severity: "Info",
            payload: new
            {
                StepId = step.Metadata.Id,
                StepName = step.Metadata.DisplayName,
                StepType = step.GetType().FullName,
                ContextType = typeof(TContext).FullName,
                BeforeType = before.GetType().FullName
            },
            context: SystemContext);
    }

    /// <summary>
    /// Emits a structured trace event indicating that a step has completed
    /// execution, including duration and context transition metadata.
    /// </summary>
    protected virtual void EmitEndEvent(TStep step, TContext before, TContext after, long durationMs)
    {
        Sink.Emit(
            eventName: "ImmutableContextBuilder.StepEnd",
            category: "ImmutableContextBuilder",
            severity: "Info",
            payload: new
            {
                StepId = step.Metadata.Id,
                StepName = step.Metadata.DisplayName,
                StepType = step.GetType().FullName,
                ContextType = typeof(TContext).FullName,
                BeforeType = before.GetType().FullName,
                AfterType = after.GetType().FullName,
                DurationMs = durationMs
            },
            context: SystemContext);
    }

    // ------------------------------------------------------------
    // INTERNAL EXECUTION
    // ------------------------------------------------------------

    private static bool TrySelectNextStep(
        HashSet<TStep> remaining,
        TContext ctx,
        out TStep? step)
    {
        step = remaining.FirstOrDefault(s => s.CanExecute(ctx));
        return step is not null;
    }

    private async Task<TContext> ExecuteStepAsync(
        TStep step,
        TContext before,
        CancellationToken ct)
    {
        EmitStartEvent(step, before);

        var sw = Stopwatch.StartNew();
        var after = await step.ExecuteAsync(before, ct);
        sw.Stop();

        EmitEndEvent(step, before, after, sw.ElapsedMilliseconds);

        return after;
    }
}
