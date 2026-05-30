using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
namespace CampFitFurDogs.Application.Authentication;

/// <summary>
/// Orchestrates the authentication callback flow using a pipeline of small steps.
/// Each step performs one responsibility and updates the shared context.
/// </summary>
public sealed class AuthCallbackService : IAuthCallbackService
{
    private readonly IEnumerable<IAuthCallbackStep> _steps;

    public AuthCallbackService(IEnumerable<IAuthCallbackStep> steps)
    {
        _steps = steps;
    }

    public async Task<AuthCallbackResult> HandleAsync(string code, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new AuthCallbackException(AuthCallbackError.MissingAuthorizationCode);

        var ctx = new AuthCallbackContext(code);

        foreach (var step in _steps)
            await step.ExecuteAsync(ctx, ct);

        return ctx.Result!;
    }
}
