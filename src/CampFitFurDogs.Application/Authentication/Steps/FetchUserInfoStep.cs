using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class FetchUserInfoStep : IAuthCallbackStep
{
    private readonly IAuthClient _client;

    public FetchUserInfoStep(IAuthClient client)
    {
        _client = client;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        var user = await _client.GetUserAsync(
            ctx.Token!,      // 1. accessToken
            ct);                   // 3. cancellation token

        if (user == null)
            throw new AuthCallbackException(AuthCallbackError.UserInfoFailure);

        ctx.User = user;
        return ctx;
    }
}
