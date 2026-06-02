using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Pipeline.Steps
;

public sealed class ValidateUserStep : IAuthCallbackStep
{
    public AuthCallbackStepMetadata Metadata =>
        new(
            "ValidateUser",
            "Validate User",
            AuthCallbackStepCategory.ValidateUser);

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireUser();

        if (string.IsNullOrWhiteSpace(ctx.User!.ExternalId))
            throw new AuthCallbackException(AuthCallbackError.MissingExternalId);

        return ctx;
    }
}
