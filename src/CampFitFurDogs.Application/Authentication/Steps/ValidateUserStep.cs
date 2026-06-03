using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class ValidateUserStep : IAuthCallbackStep
{
    public AuthCallbackStepMetadata Metadata =>
        new("ValidateUser", "Validate User");

    public bool CanExecute(AuthCallbackContext ctx)
        => ctx.User is not null;

    public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireUser();

        if (string.IsNullOrWhiteSpace(ctx.User!.ExternalId))
            throw new AuthCallbackException(AuthCallbackError.MissingExternalId);

        return Task.FromResult(ctx);
    }
}
