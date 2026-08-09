namespace Frank.Identity.Application.Abstractions.Users.GetUserById;

/// <summary>
/// Defines the contract for retrieving a user based on their internal unique
/// identifier.
/// <para>
/// This reader is part of the internal‑ID lookup pipeline. It is responsible for
/// resolving a user record using the system‑assigned <see cref="Guid"/> user ID,
/// typically created during onboarding or external‑ID mapping.
/// </para>
/// <para>
/// The reader returns a <see cref="GetUserByIdResponse"/> when a matching user
/// exists, or <c>null</c> when no user is associated with the provided ID.
/// </para>
/// </summary>
/// <remarks>
/// Implementations may apply infrastructure‑level behaviors such as:
/// <list type="bullet">
/// <item><description>Database or cache lookup</description></item>
/// <item><description>Normalization or canonicalization of identifiers</description></item>
/// <item><description>Transactional consistency (via UnitOfWork)</description></item>
/// </list>
/// The reader performs no domain logic beyond lookup; it simply returns the
/// minimal user information required by upstream application flows.
/// </remarks>
public interface IGetUserByIdReader
{
    /// <summary>
    /// Retrieves a user using their internal unique identifier.
    /// <para>
    /// Implementations must ensure secure lookup and return <c>null</c> when no
    /// matching user exists.
    /// </para>
    /// </summary>
    /// <param name="UserId">
    /// The internal <see cref="Guid"/> identifier of the user.
    /// </param>
    /// <param name="cancellationToken">
    /// A token allowing the caller to cancel the read operation.
    /// </param>
    /// <returns>
    /// A <see cref="GetUserByIdResponse"/> containing the user's basic profile
    /// information, or <c>null</c> if no user is associated with the ID.
    /// </returns>
    Task<GetUserByIdResponse?> ReadAsync(
        Guid UserId,
        CancellationToken cancellationToken);
}
