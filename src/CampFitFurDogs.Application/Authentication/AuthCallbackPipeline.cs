using CampFitFurDogs.Application.Authentication.Steps;

namespace CampFitFurDogs.Application.Authentication;

public class AuthCallbackPipeline
{
    private readonly IEnumerable<IAuthCallbackStep> _steps;

    public AuthCallbackPipeline(IEnumerable<IAuthCallbackStep> steps)
    {
        _steps = steps;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        foreach (var step in _steps)
        {
            ctx = await step.ExecuteAsync(ctx, ct);
        }

        return ctx;
    }
}
