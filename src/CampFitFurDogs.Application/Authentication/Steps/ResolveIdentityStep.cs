using CampFitFurDogs.Application.Abstractions.Identity;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class ResolveIdentityStep : IAuthCallbackStep
{
    private readonly IIdentityResolver _resolver;

    public ResolveIdentityStep(IIdentityResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        // ctx.Profile is now an OidcUserProfile
        ctx.CustomerId = await _resolver.ResolveAsync(
            ctx.User!,   // pass the entire profile
            ct);
    }
}
