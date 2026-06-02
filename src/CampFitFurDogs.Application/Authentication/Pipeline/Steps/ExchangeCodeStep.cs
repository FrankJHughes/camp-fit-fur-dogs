using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Pipeline.Steps
;

public sealed class ExchangeCodeStep : IAuthCallbackStep
{
    private readonly IAuthClient _client;

    public AuthCallbackStepMetadata Metadata =>
        new(
            "ExchangeCode",
            "Exchange Code",
            AuthCallbackStepCategory.ExchangeCode);

    public ExchangeCodeStep(IAuthClient client)
    {
        _client = client;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireCode();

        var token = await _client.ExchangeAsync(
            ctx.Code,
            ct);

        return ctx with { Token = token };
    }
}
