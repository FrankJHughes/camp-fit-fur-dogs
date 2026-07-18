using Frank.Identity.Application.Abstractions;
using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;

namespace Frank.Identity.Application.Callback.Save.Steps;

public sealed class ResolveUserStep
    : IImmutableContextBuildStep<CallbackSaveContext>
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

    public bool CanExecute(CallbackSaveContext ctx)
        => ctx.UserId is null; // only run once, at the start

    public async Task<CallbackSaveContext> ExecuteAsync(
        CallbackSaveContext ctx,
        CancellationToken ct)
    {
        var external = ctx.External;
        var userId = await _identityResolver.ResolveAsync(external, ct);

        return ctx with { UserId = userId };
    }
}
