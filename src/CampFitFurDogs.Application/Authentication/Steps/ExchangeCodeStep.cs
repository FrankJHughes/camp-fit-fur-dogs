using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class ExchangeCodeStep : IAuthCallbackStep
{
    private readonly IAuthClient _client;

    public ExchangeCodeStep(IAuthClient client)
    {
        _client = client;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        var token = await _client.ExchangeAsync(
            ctx.Code,
            ct);
        ctx.Token = token;
        return ctx;
    }
}
