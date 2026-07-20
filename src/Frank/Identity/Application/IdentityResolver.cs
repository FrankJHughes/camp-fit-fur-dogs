using Frank.Identity.Application.Abstractions;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;
using Frank.Identity.Domain.Users;

namespace Frank.Identity.Application;

public sealed class IdentityResolver : IIdentityResolver
{
    private readonly IGetUserByExternalIdReader _reader;
    private readonly ICreateUserWriter _writer;

    public IdentityResolver(
        IGetUserByExternalIdReader reader,
        ICreateUserWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<Guid> ResolveAsync(
        CallbackOidcContextBuilderResult oidcCallbackResult,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Domain will validate externalUserId, firstName, lastName, email
        // Command validator will validate identity-source semantics
        // Request validator will validate syntactic rules

        var existing = await _reader.ReadAsync(oidcCallbackResult.SubjectId, cancellationToken);
        if (existing is not null)
            return existing.Id;

        var user = User.Create(
            FirstName.From(oidcCallbackResult.GivenName!),
            LastName.From(oidcCallbackResult.FamilyName!),
            Email.From(oidcCallbackResult.Email!),
            ExternalId.From(oidcCallbackResult.SubjectId));

        await _writer.WriteAsync(user, cancellationToken);
        return user.Id.Value;
    }
}
