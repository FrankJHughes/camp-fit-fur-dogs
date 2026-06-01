using Microsoft.Extensions.Options;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class BuildRedirectStep : IAuthCallbackStep
{
    private readonly OidcOptions _options;

    public BuildRedirectStep(IOptions<OidcOptions> options)
    {
        _options = options.Value;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireSessionCookie();
        ctx.RequireSession();
        ctx.RedirectUrl = _options.PostLoginRedirectUrl;
        return ctx;
    }
}
