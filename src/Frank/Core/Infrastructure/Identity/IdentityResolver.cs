using Frank.Core.Application.Abstractions.Command;
using Frank.Identity.Abstractions;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Application.Abstractions.Users.FindUserByExternalId;

namespace Frank.Core.Infrastructure.Identity;

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
        OidcCallbackContextBuilderResult oidcCallbackResult,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Domain will validate externalUserId, firstName, lastName, email
        // Command validator will validate identity-source semantics
        // Request validator will validate syntactic rules

        var existing = await _reader.FindByExternalIdAsync(oidcCallbackResult.SubjectId, cancellationToken);
        if (existing is not null)
            return existing.Id;

        // Create a new user via application command
        var command = new CreateUserCommand(
            ExternalId: oidcCallbackResult.SubjectId,
            FirstName: oidcCallbackResult.GivenName!,
            LastName: oidcCallbackResult.FamilyName!,
            Email: oidcCallbackResult.Email!);

        var newId = await _dispatcher.DispatchAsync(command, cancellationToken);
        return newId;
    }
}
