using Frank.Abstractions.Command;
using Frank.Abstractions.Identity;
using Frank.Abstractions.Identity.Callback;
using Frank.Application.Abstractions.Users.CreateUser;
using Frank.Application.Abstractions.Users.FindUserByExternalId;

namespace Frank.Infrastructure.Identity;

public sealed class IdentityResolver : IIdentityResolver
{
    private readonly IFindUserByExternalIdReader _reader;
    private readonly ICommandDispatcher _dispatcher;

    public IdentityResolver(
        IFindUserByExternalIdReader reader,
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

        // Create a new user via application command
        var command = new CreateUserCommand(
            ExternalId: frankAuthCallbackResult.SubjectId,
            FirstName: frankAuthCallbackResult.GivenName!,
            LastName: frankAuthCallbackResult.FamilyName!,
            Email: frankAuthCallbackResult.Email!);

        var newId = await _dispatcher.DispatchAsync(command, cancellationToken);
        return newId;
    }
}
