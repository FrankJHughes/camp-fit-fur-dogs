using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Identity;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class ResolveIdentityStep : IAuthCallbackStep
{
    private readonly IIdentityResolver _resolver;

    public AuthCallbackStepMetadata Metadata =>
        new("ResolveIdentity", "Resolve Identity");

    public ResolveIdentityStep(IIdentityResolver resolver)
    {
        _resolver = resolver;
    }

    public bool CanExecute(AuthCallbackContext ctx)
        => ctx.User is not null;

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireUser();

        var resolved = await _resolver.ResolveAsync(ctx.User!, ct);

        return ctx with { CustomerId = resolved };
    }
}
