using Microsoft.Extensions.Options;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;

namespace CampFitFurDogs.Application.Authentication.Pipeline.Steps;

public sealed class BuildRedirectStep : IAuthCallbackStep
{
    private readonly OidcOptions _options;

    public AuthCallbackStepMetadata Metadata => new("BuildRedirect", "Build Redirect", AuthCallbackStepCategory.BuildRedirect);

    public BuildRedirectStep(IOptions<OidcOptions> options)
    {
        _options = options.Value;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireSessionCookie();
        ctx.RequireSession();

        if (string.IsNullOrWhiteSpace(_options.PostLoginRedirectUrl))
            throw new InvalidOperationException("PostLoginRedirectUrl must be configured.");

        return ctx with { RedirectUrl = _options.PostLoginRedirectUrl };
    }
}
