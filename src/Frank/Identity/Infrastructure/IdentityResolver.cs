using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Identity.Application.Abstractions;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;

namespace Frank.Identity.Infrastructure;

public sealed class IdentityResolver : IIdentityResolver
{
    private readonly IGetUserByExternalIdReader _reader;
    private readonly ICommandDispatcher _dispatcher;

    public IdentityResolver(
        IGetUserByExternalIdReader reader,
        ICommandDispatcher dispatcher)
    {
        _reader = reader;
        _dispatcher = dispatcher;
    }

    public async Task<Guid> ResolveAsync(
        CallbackOidcContextBuilderResult oidcCallbackResult,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Domain will validate externalUserId, firstName, lastName, email
        // Command validator will validate identity-source semantics
        // Request validator will validate syntactic rules

        var existing = await _reader.GetByExternalIdAsync(oidcCallbackResult.SubjectId, cancellationToken);
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
