using Frank.Abstractions.ImmutableContext;

namespace Frank.Authentication.Callback.Oidc.Steps;

public sealed class FetchUserInfoStep : IImmutableContextBuildStep<OidcAuthCallbackContext>
{
    private readonly IOidcUserInfoClient _client;

    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.fetch-userinfo", "Fetch OIDC UserInfo");

    public FetchUserInfoStep(IOidcUserInfoClient client)
    {
        _client = client;
    }

    public bool CanExecute(OidcAuthCallbackContext ctx)
        => ctx.AccessToken is not null;

    public async Task<OidcAuthCallbackContext> ExecuteAsync(
        OidcAuthCallbackContext ctx,
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
