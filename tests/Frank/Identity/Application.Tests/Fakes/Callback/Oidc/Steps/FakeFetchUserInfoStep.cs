using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Oidc;

namespace Frank.Identity.Application.Tests.Fakes.Callback.Oidc.Steps;

public sealed class FakeFetchUserInfoStep : IImmutableContextBuildStep<CallbackOidcContext>
{
    private readonly OidcUserInfo _info;

    public FakeFetchUserInfoStep(OidcUserInfo info)
    {
        _info = info;
    }

    public bool CanExecute(CallbackOidcContext ctx) => true;

    public Task<CallbackOidcContext> ExecuteAsync(
        CallbackOidcContext ctx,
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
