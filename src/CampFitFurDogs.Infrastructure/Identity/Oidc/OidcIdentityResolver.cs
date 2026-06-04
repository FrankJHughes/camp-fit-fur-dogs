using CampFitFurDogs.Application.Abstractions.Customers.FindCustomerByExternalId;
using Frank.Abstractions;
using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Application.Abstractions.Identity;
using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Infrastructure.Identity.Oidc;

public sealed class OidcIdentityResolver : IIdentityResolver
{
    private readonly IFindCustomerByExternalIdReader _reader;
    private readonly ICommandDispatcher _dispatcher;

    public OidcIdentityResolver(
        IFindCustomerByExternalIdReader reader,
        ICommandDispatcher dispatcher)
    {
        _reader = reader;
        _dispatcher = dispatcher;
    }

    public async Task<Guid> ResolveAsync(
        AuthUser profile,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Domain will validate externalUserId, firstName, lastName, email
        // Command validator will validate identity-source semantics
        // Request validator will validate syntactic rules

        var existing = await _reader.FindByExternalIdAsync(profile.ExternalId, cancellationToken);
        if (existing is not null)
            return existing.Id;

        // Create a new customer via application command
        var command = new CreateCustomerCommand(
            ExternalAuthProviderId: profile.ExternalId,
            FirstName: profile.GivenName,
            LastName: profile.FamilyName,
            Email: profile.Email);

        var newId = await _dispatcher.DispatchAsync(command, cancellationToken);
        return newId;
    }
}
