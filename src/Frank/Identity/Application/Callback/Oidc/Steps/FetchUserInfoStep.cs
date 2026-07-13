using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Callback.Oidc.Steps;

public sealed class FetchUserInfoStep : IImmutableContextBuildStep<OidcCallbackContext>
{
    private readonly IOidcUserInfoClient _client;

    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.fetch-userinfo", "Fetch OIDC UserInfo");

    public FetchUserInfoStep(IOidcUserInfoClient client)
    {
        _client = client;
    }

    public bool CanExecute(OidcCallbackContext ctx)
        => ctx.AccessToken is not null;

    public async Task<OidcCallbackContext> ExecuteAsync(
        OidcCallbackContext ctx,
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
