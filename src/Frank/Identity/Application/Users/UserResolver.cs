using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Users;
using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;
using Frank.Identity.Domain.Users;

namespace Frank.Identity.Application.Users;

/// <summary>
/// Resolves a user from an external identity provider subject ID.
/// <para>
/// This component is used during the Save pipeline after the OIDC callback
/// pipeline has produced a validated <see cref="CallbackOidcContextBuilderResult"/>.
/// </para>
/// <para>
/// The resolver performs the following steps:
/// </para>
/// <list type="bullet">
/// <item><description>Checks whether a user already exists for the external subject ID</description></item>
/// <item><description>If found, returns the existing user ID</description></item>
/// <item><description>If not found, constructs a new <see cref="User"/> domain entity</description></item>
/// <item><description>Persists the new user via <see cref="ICreateUserWriter"/></description></item>
/// </list>
/// <para>
/// Domain invariants are enforced by value objects such as <see cref="FirstName"/>,
/// <see cref="LastName"/>, <see cref="Email"/>, and <see cref="ExternalId"/>.
/// </para>
/// </summary>
public sealed class UserResolver : IUserResolver
{
    private readonly IGetUserByExternalIdReader _reader;
    private readonly ICreateUserWriter _writer;

    /// <summary>
    /// Creates a new <see cref="UserResolver"/> using the provided reader and writer.
    /// </summary>
    /// <param name="reader">
    /// The reader responsible for retrieving users by external identity provider subject ID.
    /// </param>
    /// <param name="writer">
    /// The writer responsible for persisting newly created users.
    /// </param>
    public UserResolver(
        IGetUserByExternalIdReader reader,
        ICreateUserWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    /// <summary>
    /// Resolves a user based on the external identity information produced by the OIDC callback pipeline.
    /// <para>
    /// If a user already exists for the given external subject ID, the existing user ID is returned.
    /// Otherwise, a new <see cref="User"/> is created using the identity information from the callback
    /// result and persisted.
    /// </para>
    /// </summary>
    /// <param name="oidcCallbackResult">
    /// The validated OIDC callback result containing subject ID, email, given name, and family name.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// The <see cref="Guid"/> identifier of the resolved or newly created user.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the cancellation token is signaled before completion.
    /// </exception>
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
