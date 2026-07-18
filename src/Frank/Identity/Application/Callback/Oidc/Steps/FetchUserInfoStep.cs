using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Oidc;

namespace Frank.Identity.Application.Callback.Oidc.Steps;

public sealed class FetchUserInfoStep : IImmutableContextBuildStep<CallbackOidcContext>
{
    private readonly IOidcUserInfoClient _client;

    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.fetch-userinfo", "Fetch OIDC UserInfo");

    public FetchUserInfoStep(IOidcUserInfoClient client)
    {
        _client = client;
    }

    public bool CanExecute(CallbackOidcContext ctx)
        => ctx.AccessToken is not null;

    public async Task<CallbackOidcContext> ExecuteAsync(
        CallbackOidcContext ctx,
        CancellationToken ct)
    {
        var info = await _client.GetUserInfoAsync(ctx.AccessToken!, ct);

        return ctx with
        {
            SubjectId = info.Subject,
            Claims = info.Claims,
            Email = info.Email,
            GivenName = info.GivenName,
            FamilyName = info.FamilyName,
            Picture = info.Picture
        };
    }
}
