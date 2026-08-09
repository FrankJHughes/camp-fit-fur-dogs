using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Abstractions.Sessions;

/// <summary>
/// Defines the contract for generating and hashing session tokens used by the
/// Identity subsystem.
/// <para>
/// A session token consists of two parts:
/// </para>
/// <list type="bullet">
/// <item><description>
/// A randomly generated plaintext token returned to the client (typically stored
/// in a secure cookie).
/// </description></item>
/// <item><description>
/// A corresponding <see cref="SessionTokenHash"/> computed from the plaintext
/// token and persisted in durable storage.
/// </description></item>
/// </list>
/// <para>
/// The plaintext token is **never persisted** and must never be logged.
/// The hashed token is used for all future lookups, validation, and revocation.
/// </para>
/// </summary>
/// <remarks>
/// Implementations of this interface must ensure:
/// <list type="bullet">
/// <item><description>Cryptographically secure random token generation</description></item>
/// <item><description>Non‑reversible hashing suitable for long‑term storage</description></item>
/// <item><description>Deterministic hashing (same input → same hash)</description></item>
/// <item><description>Zero exposure of the plaintext token beyond creation time</description></item>
/// </list>
/// </remarks>
public interface ISessionTokenGenerator
{
    /// <summary>
    /// Generates a new session token and its corresponding hash.
    /// <para>
    /// The returned <see cref="GeneratedSessionToken"/> contains both the
    /// plaintext token (for client use) and the hashed token (for storage).
    /// </para>
    /// </summary>
    /// <returns>
    /// A <see cref="GeneratedSessionToken"/> containing the plaintext token and
    /// its secure hash.
    /// </returns>
    GeneratedSessionToken Generate();

    /// <summary>
    /// Computes a secure, non‑reversible hash from the provided plaintext token.
    /// <para>
    /// This method is used when validating or reconstructing the hash for
    /// comparison, such as during session retrieval or revocation.
    /// </para>
    /// </summary>
    /// <param name="plaintextToken">
    /// The raw session token provided by the client.
    /// </param>
    /// <returns>
    /// A <see cref="SessionTokenHash"/> representing the secure hash of the
    /// plaintext token.
    /// </returns>
    SessionTokenHash Hash(string plaintextToken);
}
