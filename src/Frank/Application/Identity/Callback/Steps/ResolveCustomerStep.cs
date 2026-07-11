using Frank.Application.Abstractions.Identity.Callback;
using Frank.Abstractions.Identity;
using Frank.Abstractions.ImmutableContext;

namespace Frank.Application.Identity.Callback.Steps;

public sealed class ResolveUserStep
    : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
{
    private readonly IIdentityResolver _identityResolver;

    public ResolveUserStep(IIdentityResolver identityResolver)
    {
        _identityResolver = identityResolver;
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(
            id: "ResolveUser",
            displayName: "Resolve User"
        );

    public bool CanExecute(ApplicationAuthCallbackContext ctx)
        => ctx.UserId is null; // only run once, at the start

    public async Task<ApplicationAuthCallbackContext> ExecuteAsync(
        ApplicationAuthCallbackContext ctx,
        CancellationToken ct)
    {
        var external = ctx.External;
        var userId = await _identityResolver.ResolveAsync(external, ct);

        return ctx with { UserId = userId };
    }
}
