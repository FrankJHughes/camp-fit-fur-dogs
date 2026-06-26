using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Application.Abstractions.Customers.FindCustomerByExternalId;
using Frank.Abstractions.Identity;
using Frank.Abstractions.Authentication.Callback;
using Frank.Abstractions.Command;

namespace CampFitFurDogs.Infrastructure.Identity;

public sealed class IdentityResolver : IIdentityResolver
{
    private readonly IFindCustomerByExternalIdReader _reader;
    private readonly ICommandDispatcher _dispatcher;

    public IdentityResolver(
        IFindCustomerByExternalIdReader reader,
        ICommandDispatcher dispatcher)
    {
        _reader = reader;
        _dispatcher = dispatcher;
    }

    public async Task<Guid> ResolveAsync(
        FrankAuthCallbackResult frankAuthCallbackResult,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Domain will validate externalUserId, firstName, lastName, email
        // Command validator will validate identity-source semantics
        // Request validator will validate syntactic rules

        var existing = await _reader.FindByExternalIdAsync(frankAuthCallbackResult.SubjectId, cancellationToken);
        if (existing is not null)
            return existing.Id;

        // Create a new customer via application command
        var command = new CreateCustomerCommand(
            ExternalId: frankAuthCallbackResult.SubjectId,
            FirstName: frankAuthCallbackResult.GivenName!,
            LastName: frankAuthCallbackResult.FamilyName!,
            Email: frankAuthCallbackResult.Email!);

        var newId = await _dispatcher.DispatchAsync(command, cancellationToken);
        return newId;
    }
}
