using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Time;
using CampFitFurDogs.Application.Authentication.Steps;

namespace CampFitFurDogs.Application.Authentication;

/// <summary>
/// Orchestrates the authentication callback flow using a pipeline of small steps.
/// Each step performs one responsibility and updates the shared context.
/// </summary>
public sealed class AuthCallbackService : IAuthCallbackService
{
    private readonly IEnumerable<IAuthCallbackStep> _steps;
    private readonly ISystemClock _clock;

    public AuthCallbackService(IEnumerable<IAuthCallbackStep> steps)
        : this(steps, new DefaultSystemClock())
    {
    }

    public AuthCallbackService(IEnumerable<IAuthCallbackStep> steps, ISystemClock clock)
    {
        _steps = steps;
        _clock = clock;
    }

    public async Task<AuthCallbackResult> HandleAsync(string code, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new AuthCallbackException(AuthCallbackError.MissingAuthorizationCode);

        // Initialize context with a deterministic timestamp
        var ctx = new AuthCallbackContext(code)
        {
            Now = _clock.UtcNow
        };

        // Execute pipeline steps in order
        foreach (var step in _steps)
            await step.ExecuteAsync(ctx, ct);

        // Ensure a result was produced
        if (ctx.Result is null)
            throw new AuthCallbackException(AuthCallbackError.MissingResult);

        return ctx.Result;
    }

    private sealed class DefaultSystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
