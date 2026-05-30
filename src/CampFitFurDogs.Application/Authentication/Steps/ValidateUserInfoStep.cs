using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class ValidateUserInfoStep : IAuthCallbackStep
{
    public Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(ctx.User!.ExternalId))
            throw new AuthCallbackException(AuthCallbackError.MissingExternalId);

        return Task.CompletedTask;
    }
}
