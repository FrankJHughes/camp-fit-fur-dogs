namespace Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;

/// <summary>
/// Defines the contract for retrieving a user based on an external identity
/// provider identifier.
/// <para>
/// This reader is part of the external‑ID lookup pipeline. It is responsible for
/// resolving an internal user record using an external identity provider’s
/// subject identifier (e.g., OIDC <c>sub</c> claim).
/// </para>
/// <para>
/// The reader returns a <see cref="GetUserByExternalIdResponse"/> when a matching
/// user exists, or <c>null</c> when no user is associated with the provided
/// external ID.
/// </para>
/// </summary>
/// <remarks>
/// Implementations may apply infrastructure‑level behaviors such as:
/// <list type="bullet">
/// <item><description>Database or cache lookup</description></item>
/// <item><description>Normalization of external IDs</description></item>
/// <item><description>Uniqueness enforcement</description></item>
/// <item><description>Transactional consistency (via UnitOfWork)</description></item>
/// </list>
/// The reader performs no domain logic beyond lookup; it simply returns the
/// internal user identifier if found.
/// </remarks>
public interface IGetUserByExternalIdReader
{
    /// <summary>
    /// Retrieves a user using their external identity provider identifier.
    /// <para>
    /// The external ID uniquely identifies the user in the context of an
    /// external identity provider (e.g., OIDC, OAuth, SSO).
    /// Implementations must ensure secure lookup and return <c>null</c> when no
    /// matching user exists.
    /// </para>
    /// </summary>
    /// <param name="externalId">
    /// The external identity provider’s unique identifier for the user.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token allowing the caller to cancel the read operation.
    /// </param>
    /// <returns>
    /// A <see cref="GetUserByExternalIdResponse"/> containing the internal user
    /// ID, or <c>null</c> if no user is associated with the external ID.
    /// </returns>
    Task<GetUserByExternalIdResponse?> ReadAsync(
        string externalId,
        CancellationToken cancellationToken);
}
