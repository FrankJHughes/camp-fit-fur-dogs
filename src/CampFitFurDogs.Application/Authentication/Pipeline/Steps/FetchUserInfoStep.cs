using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Pipeline.Steps
;

public sealed class FetchUserStep : IAuthCallbackStep
{
    private readonly IAuthClient _client;
    public AuthCallbackStepMetadata Metadata =>
        new(
            "FetchUser",
            "Fetch User",
            AuthCallbackStepCategory.FetchUser);

    public FetchUserStep(IAuthClient client)
    {
        _client = client;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireToken();

        var user = await _client.GetUserAsync(ctx.Token!, ct);

        if (user == null)
            throw new AuthCallbackException(AuthCallbackError.UserInfoFailure);

        return ctx with { User = user };
    }
}
