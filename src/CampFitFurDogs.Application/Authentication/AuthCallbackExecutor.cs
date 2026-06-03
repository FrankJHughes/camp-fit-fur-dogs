using System.Diagnostics;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication.Diagnostics;

namespace CampFitFurDogs.Application.Authentication;

public sealed class AuthCallbackExecutor
{
    private readonly IReadOnlyList<IAuthCallbackStep> _steps;
    private readonly Action<AuthCallbackDiagnosticEvent>? _trace;

    public AuthCallbackExecutor(
        IEnumerable<IAuthCallbackStep> steps,
        Action<AuthCallbackDiagnosticEvent>? trace = null)
    {
        _steps = [.. steps];
        _trace = trace;
    }

    public async Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        var remaining = new HashSet<IAuthCallbackStep>(_steps);

        while (TrySelectNextStep(remaining, ctx, out var step))
        {
            var before = ctx;

            ctx = await ExecuteStep(
                step!,
                before,
                emitBefore: EmitStartEvent,
                emitAfter: EmitEndEvent,
                ct);

            AssertValidTransition(step!, before, ctx);

            remaining.Remove(step!);
        }
    }

    // ------------------------------------------------------------
    // Step Selection
    // ------------------------------------------------------------

    private static bool TrySelectNextStep(
        HashSet<IAuthCallbackStep> remaining,
        AuthCallbackContext ctx,
        out IAuthCallbackStep? step)
    {
        step = remaining.FirstOrDefault(s => s.CanExecute(ctx));
        return step is not null;
    }

    // ------------------------------------------------------------
    // Diagnostics Emitters
    // ------------------------------------------------------------

    private void EmitStartEvent(IAuthCallbackStep step, AuthCallbackContext before)
    {
        _trace?.Invoke(new AuthCallbackDiagnosticEvent(
            StepId: step.Metadata.Id,
            StepName: step.Metadata.DisplayName,
            Phase: "Start",
            DurationMs: null,
            Before: before,
            After: before));
    }

    private void EmitEndEvent(
        IAuthCallbackStep step,
        AuthCallbackContext before,
        AuthCallbackContext after,
        long durationMs)
    {
        _trace?.Invoke(new AuthCallbackDiagnosticEvent(
            StepId: step.Metadata.Id,
            StepName: step.Metadata.DisplayName,
            Phase: "End",
            DurationMs: durationMs,
            Before: before,
            After: after));
    }

    // ------------------------------------------------------------
    // Execution (now with injected emitters)
    // ------------------------------------------------------------

    private static async Task<AuthCallbackContext> ExecuteStep(
        IAuthCallbackStep step,
        AuthCallbackContext before,
        Action<IAuthCallbackStep, AuthCallbackContext>? emitBefore,
        Action<IAuthCallbackStep, AuthCallbackContext, AuthCallbackContext, long>? emitAfter,
        CancellationToken ct)
    {
        emitBefore?.Invoke(step, before);

        var sw = Stopwatch.StartNew();
        var after = await step.ExecuteAsync(before, ct);
        sw.Stop();

        emitAfter?.Invoke(step, before, after, sw.ElapsedMilliseconds);

        return after;
    }

    // ------------------------------------------------------------
    // Invariant Enforcement
    // ------------------------------------------------------------

    private static void AssertValidTransition(
        IAuthCallbackStep step,
        AuthCallbackContext before,
        AuthCallbackContext? after)
    {
        if (after is null)
            throw new InvalidOperationException(
                $"Pipeline step '{step.Metadata.Id}' returned null context.");

        AssertImmutable(step, before, after);
        AssertNoClearing(step, before, after);
    }

    private static void AssertImmutable(
        IAuthCallbackStep step,
        AuthCallbackContext before,
        AuthCallbackContext after)
    {
        if (after.Code != before.Code)
            throw new InvalidOperationException(
                $"Pipeline step '{step.Metadata.Id}' attempted to modify immutable field 'Code'.");

        if (after.Now != before.Now)
            throw new InvalidOperationException(
                $"Pipeline step '{step.Metadata.Id}' attempted to modify immutable field 'Now'.");
    }

    private static void AssertNoClearing(
        IAuthCallbackStep step,
        AuthCallbackContext before,
        AuthCallbackContext after)
    {
        EnsureNotCleared(before.Token, after.Token, step, "Token");
        EnsureNotCleared(before.User, after.User, step, "User");
        EnsureNotCleared(before.CustomerId, after.CustomerId, step, "CustomerId");
        EnsureNotCleared(before.TokenHash, after.TokenHash, step, "TokenHash");
        EnsureNotCleared(before.Session, after.Session, step, "Session");
        EnsureNotCleared(before.SessionCookie, after.SessionCookie, step, "SessionCookie");
        EnsureNotCleared(before.RedirectUrl, after.RedirectUrl, step, "RedirectUrl");
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
