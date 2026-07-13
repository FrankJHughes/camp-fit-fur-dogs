using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Callback.Oidc;

namespace Frank.Tests.Fakes.Authentication.Callback.Oidc.Steps;

public sealed class FakeFetchUserInfoStep : IImmutableContextBuildStep<OidcCallbackContext>
{
    private readonly OidcUserInfo _info;

    public FakeFetchUserInfoStep(OidcUserInfo info)
    {
        _info = info;
    }

    public bool CanExecute(OidcCallbackContext ctx) => true;

    public Task<OidcCallbackContext> ExecuteAsync(
        OidcCallbackContext ctx,
        CancellationToken ct)
    {
        return Task.FromResult(ctx with
        {
            SubjectId = _info.Subject,
            Claims = _info.Claims,
            Email = _info.Email,
            GivenName = _info.GivenName,
            FamilyName = _info.FamilyName,
            Picture = _info.Picture
        });
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("FakeFetchUserInfo", "Fake Fetch UserInfo");
}
