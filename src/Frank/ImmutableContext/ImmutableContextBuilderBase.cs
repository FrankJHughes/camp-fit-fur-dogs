using System.Diagnostics;
using Frank.Abstractions.ImmutableContext;
using Frank.Abstractions.Observability;

namespace Frank.ImmutableContext;

public abstract class ImmutableContextBuilderBase<TContext, TStep>
    where TContext : ImmutableContextBase
    where TStep : IImmutableContextBuildStep<TContext>
{
    private readonly IReadOnlyList<TStep> _steps;
    protected IObservabilitySink Sink { get; }
    protected IObservabilityContext SystemContext { get; }

    protected ImmutableContextBuilderBase(
        IEnumerable<TStep> steps,
        IObservabilitySink sink,
        IObservabilityContext systemContext)
    {
        _steps = steps.ToList();
        Sink = sink;
        SystemContext = systemContext;
    }

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

    protected abstract void AssertValidTransition(TStep step, TContext before, TContext after);

    // ------------------------------------------------------------
    // OBSERVABILITY HOOKS
    // ------------------------------------------------------------
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
