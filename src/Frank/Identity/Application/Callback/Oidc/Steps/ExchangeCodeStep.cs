using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Oidc;

public sealed class ExchangeCodeStep : IImmutableContextBuildStep<CallbackOidcContext>
{
    private readonly IOidcTokenClient _client;

    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.exchange-code", "Exchange Authorization Code");

    public ExchangeCodeStep(IOidcTokenClient client)
    {
        _client = client;
    }

    public bool CanExecute(CallbackOidcContext ctx)
        => ctx.Code is not null;

    public async Task<CallbackOidcContext> ExecuteAsync(
        CallbackOidcContext ctx,
        CancellationToken ct)
    {
        var tokens = await _client.ExchangeCodeAsync(ctx.Code!, ct);

        return ctx with
        {
            AccessToken = tokens.AccessToken,
            IdToken = tokens.IdToken   // ✔ FIXED
        };
    }
}
