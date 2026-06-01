using System.Diagnostics;

namespace CampFitFurDogs.Application.Authentication.Pipeline;

public sealed class AuthCallbackPipeline
{
    private readonly IEnumerable<IAuthCallbackStep> _steps;
    private readonly AuthCallbackTrace? _trace;

    public AuthCallbackPipeline(
        IEnumerable<IAuthCallbackStep> steps,
        AuthCallbackTrace? trace = null)
    {
        _steps = steps;
        _trace = trace;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(
        AuthCallbackContext ctx,
        CancellationToken ct)
    {
        foreach (var step in _steps)
        {
            var before = ctx;

            // Emit START event
            _trace?.Invoke(new PipelineDiagnosticEvent(
                StepId: step.Metadata.Id,
                StepName: step.Metadata.DisplayName,
                Phase: "Start",
                DurationMs: null,
                Before: before,
                After: before
            ));

            var sw = Stopwatch.StartNew();
            var after = await step.ExecuteAsync(before, ct);
            sw.Stop();

            // Emit END event
            _trace?.Invoke(new PipelineDiagnosticEvent(
                StepId: step.Metadata.Id,
                StepName: step.Metadata.DisplayName,
                Phase: "End",
                DurationMs: sw.ElapsedMilliseconds,
                Before: before,
                After: after
            ));

            ctx = after;
        }

        return ctx;
    }
}
