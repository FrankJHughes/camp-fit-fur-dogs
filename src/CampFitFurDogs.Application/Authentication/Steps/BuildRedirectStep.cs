using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using Microsoft.Extensions.Options;

public sealed class BuildRedirectStep : IAuthCallbackStep
{
    private readonly string _redirectUrl;

    public AuthCallbackStepMetadata Metadata =>
        new("BuildRedirect", "Build Redirect");

    public BuildRedirectStep(IOptions<OidcOptions> options)
    {
        _redirectUrl = options.Value.PostLoginRedirectUrl
            ?? throw new InvalidOperationException("PostLoginRedirectUrl must be configured.");
    }

    public bool CanExecute(AuthCallbackContext ctx)
        => ctx.SessionCookie is not null && ctx.Session is not null;

    public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireSessionCookie();
        ctx.RequireSession();

        return Task.FromResult(ctx with { RedirectUrl = _redirectUrl });
    }
}
