using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Customers;
using Frank.Abstractions.Time;

namespace CampFitFurDogs.Application.Authentication;

/// <summary>
/// Orchestrates the authentication callback flow using a pipeline of small steps.
/// Each step performs one responsibility and updates the shared context.
/// </summary>
public sealed class AuthCallbackService : IAuthCallbackService
{
    private readonly AuthCallbackExecutor _executor;
    private readonly IClock _clock;

    public AuthCallbackService(AuthCallbackExecutor executor, IClock clock)
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

}
