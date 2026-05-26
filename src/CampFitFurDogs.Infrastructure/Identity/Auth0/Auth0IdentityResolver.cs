using CampFitFurDogs.Application.Abstractions.Customers.FindCustomerByExternalId;
using SharedKernel.Abstractions;
using CampFitFurDogs.Application.Abstractions.Identity.External;
using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;

namespace CampFitFurDogs.Infrastructure.Identity.Auth0;

public sealed class Auth0IdentityResolver : IExternalIdentityResolver
{
    private readonly IFindCustomerByExternalIdReader _reader;
    private readonly ICommandDispatcher _dispatcher;

    public Auth0IdentityResolver(
        IFindCustomerByExternalIdReader reader,
        ICommandDispatcher dispatcher)
    {
        _reader = reader;
        _dispatcher = dispatcher;
    }

    public async Task<Guid> ResolveAsync(
        string externalUserId,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Domain will validate externalUserId, firstName, lastName, email
        // Command validator will validate identity-source semantics
        // Request validator will validate syntactic rules

        var existing = await _reader.FindByExternalIdAsync(externalUserId, cancellationToken);
        if (existing is not null)
            return existing.Id;

        // Create a new customer via application command
        var command = new CreateCustomerCommand(
            ExternalAuthProviderId: externalUserId,
            FirstName: firstName,
            LastName: lastName,
            Email: email);

        var newId = await _dispatcher.DispatchAsync(command, cancellationToken);
        return newId;
    }
}
