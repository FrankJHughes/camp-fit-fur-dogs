using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class ValidateUserInfoStep : IAuthCallbackStep
{
    public StepMetadata Metadata =>
        new("ValidateUserInfo", "Validate User Information");

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireUser();

        if (string.IsNullOrWhiteSpace(ctx.User!.ExternalId))
            throw new AuthCallbackException(AuthCallbackError.MissingExternalId);

        return ctx;
    }
}
