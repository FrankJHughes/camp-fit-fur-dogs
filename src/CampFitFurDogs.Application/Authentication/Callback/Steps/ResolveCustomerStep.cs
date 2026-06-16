using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.Application.Abstractions.Identity;
using Frank.Abstractions.ImmutableContext;

namespace CampFitFurDogs.Application.Authentication.Callback.Steps;

public sealed class ResolveCustomerStep
    : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
{
    private readonly IIdentityResolver _identityResolver;

    public ResolveCustomerStep(IIdentityResolver identityResolver)
    {
        _identityResolver = identityResolver;
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(
            id: "ResolveCustomer",
            displayName: "Resolve Customer"
        );

    public bool CanExecute(ApplicationAuthCallbackContext ctx)
        => ctx.CustomerId is null; // only run once, at the start

    public async Task<ApplicationAuthCallbackContext> ExecuteAsync(
        ApplicationAuthCallbackContext ctx,
        CancellationToken ct)
    {
        var external = ctx.External;
        var customerId = await _identityResolver.ResolveAsync(external, ct);

        return ctx with { CustomerId = customerId };
    }
}
