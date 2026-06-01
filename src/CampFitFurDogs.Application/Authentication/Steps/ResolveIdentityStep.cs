using CampFitFurDogs.Application.Abstractions.Identity;
using CampFitFurDogs.Application.Authentication;

public sealed class ResolveIdentityStep : IAuthCallbackStep
{
    private readonly IIdentityResolver _resolver;

    public ResolveIdentityStep(IIdentityResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireUser();

        Guid? resolved = await _resolver.ResolveAsync(ctx.User!, ct);

        if (resolved is null)
            throw new InvalidOperationException("Unable to resolve customer identity.");

        return ctx with { CustomerId = resolved.Value };
    }
}
