using Microsoft.Extensions.Options;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class BuildRedirectStep : IAuthCallbackStep
{
    private readonly OidcOptions _options;

    public BuildRedirectStep(IOptions<OidcOptions> options)
    {
        _options = options.Value;
    }

    public Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.Result = ctx.Result!.WithRedirect(_options.PostLoginRedirectUrl);
        return Task.CompletedTask;
    }
}
