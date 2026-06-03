using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Time;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Authentication;

/// <summary>
/// Orchestrates the authentication callback flow using a pipeline of small steps.
/// Each step performs one responsibility and updates the shared context.
/// </summary>
public sealed class AuthCallbackService : IAuthCallbackService
{
    private readonly AuthCallbackExecutor _executor;
    private readonly ISystemClock _clock;

    public AuthCallbackService(AuthCallbackExecutor executor)
        : this(executor, new DefaultSystemClock())
    {
    }

    public AuthCallbackService(AuthCallbackExecutor executor, ISystemClock clock)
    {
        _executor = executor;
        _clock = clock;
    }

    public async Task<AuthCallbackResult> HandleAsync(string code, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new AuthCallbackException(AuthCallbackError.MissingAuthorizationCode);

        var ctx = new AuthCallbackContext(code, Now: _clock.UtcNow);

        await _executor.ExecuteAsync(ctx, ct);

        ctx.RequireCustomerId();
        ctx.RequireSession();
        ctx.RequireSessionCookie();
        ctx.RequireRedirectUrl();

        return new AuthCallbackResult(
            CustomerId.From(ctx.CustomerId!.Value),
            ctx.SessionCookie!,
            ctx.RedirectUrl!
        );

    }

    private sealed class DefaultSystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
