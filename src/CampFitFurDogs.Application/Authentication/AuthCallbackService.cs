using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Time;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Authentication;

/// <summary>
/// Orchestrates the authentication callback flow using a pipeline of small steps.
/// Each step performs one responsibility and updates the shared context.
/// </summary>
public sealed class AuthCallbackService : IAuthCallbackService
{
    private readonly AuthCallbackPipeline _pipeline;
    private readonly ISystemClock _clock;

    public AuthCallbackService(AuthCallbackPipeline pipeline)
        : this(pipeline, new DefaultSystemClock())
    {
    }

    public AuthCallbackService(AuthCallbackPipeline pipeline, ISystemClock clock)
    {
        _pipeline = pipeline;
        _clock = clock;
    }

    public async Task<AuthCallbackResult> HandleAsync(string code, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new AuthCallbackException(AuthCallbackError.MissingAuthorizationCode);

        // Initialize context with a deterministic timestamp
        var initial = new AuthCallbackContext(code)
        {
            Now = _clock.UtcNow
        };

        var final = await _pipeline.ExecuteAsync(initial, ct);

        final.RequireCustomerId();
        final.RequireSession();
        final.RequireSessionCookie();
        final.RequireRedirectUrl();

        return new AuthCallbackResult(
            CustomerId.From(final.CustomerId!.Value),
            final.SessionCookie!,
            final.RedirectUrl!
        );

    }

    private sealed class DefaultSystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
