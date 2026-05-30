using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class CreateSessionCookieStep : IAuthCallbackStep
{
    public Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.Result = AuthCallbackResult.CreateSuccess(ctx.CustomerId!.Value);
        return Task.CompletedTask;
    }
}
