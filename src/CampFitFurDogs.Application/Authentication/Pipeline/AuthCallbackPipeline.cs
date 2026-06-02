using System.Diagnostics;
using CampFitFurDogs.Application.Authentication.Pipeline.Diagnostics;

namespace CampFitFurDogs.Application.Authentication.Pipeline;

public sealed class AuthCallbackPipeline
{
    private readonly IReadOnlyList<IAuthCallbackStep> _steps;
    private readonly AuthCallbackTrace? _trace;

    public AuthCallbackPipeline(
        IEnumerable<IAuthCallbackStep> steps,
        AuthCallbackTrace? trace = null)
    {
        _steps = steps is IReadOnlyList<IAuthCallbackStep> list
            ? list
            : steps.ToList();

        _trace = trace;
    }

    // Expose ordered steps for tests and diagnostics
    public IReadOnlyList<IAuthCallbackStep> Steps => _steps;

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

            // -------------------------
            // Pipeline Invariants
            // -------------------------

            if (after is null)
                throw new InvalidOperationException(
                    $"Pipeline step '{step.Metadata.Id}' returned null context.");

            // Immutable fields
            if (after.Code != before.Code)
                throw new InvalidOperationException(
                    $"Pipeline step '{step.Metadata.Id}' attempted to modify immutable field 'Code'.");

            if (after.Now != before.Now)
                throw new InvalidOperationException(
                    $"Pipeline step '{step.Metadata.Id}' attempted to modify immutable field 'Now'.");

            // No field may be cleared once set
            EnsureNotCleared(before.Token, after.Token, step, "Token");
            EnsureNotCleared(before.User, after.User, step, "User");
            EnsureNotCleared(before.CustomerId, after.CustomerId, step, "CustomerId");
            EnsureNotCleared(before.TokenHash, after.TokenHash, step, "TokenHash");
            EnsureNotCleared(before.Session, after.Session, step, "Session");
            EnsureNotCleared(before.SessionCookie, after.SessionCookie, step, "SessionCookie");
            EnsureNotCleared(before.RedirectUrl, after.RedirectUrl, step, "RedirectUrl");

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

    private static void EnsureNotCleared<T>(
        T before,
        T after,
        IAuthCallbackStep step,
        string fieldName)
    {
        if (before is not null && after is null)
        {
            throw new InvalidOperationException(
                $"Pipeline step '{step.Metadata.Id}' cleared previously set field '{fieldName}'.");
        }
    }
}
