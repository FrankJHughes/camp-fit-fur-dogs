using System.Security.Cryptography;
using System.Text;
using Frank.Identity.Application.Abstractions.Sessions;
using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Sessions;

/// <summary>
/// Generates secure session tokens and their corresponding SHA‑256 hashes.
/// <para>
/// This component is responsible for producing:
/// </para>
/// <list type="bullet">
/// <item><description>A cryptographically secure plaintext session token (256‑bit, hex‑encoded)</description></item>
/// <item><description>A SHA‑256 hash of the token for persistent storage</description></item>
/// </list>
/// <para>
/// The plaintext token is intended for cookie issuance, while the hashed token
/// is stored in the database and used for session lookup and validation.
/// </para>
/// </summary>
public sealed class SessionTokenGenerator : ISessionTokenGenerator
{
    /// <summary>
    /// Generates a new secure session token consisting of:
    /// <list type="bullet">
    /// <item><description>A 256‑bit random plaintext token (hex‑encoded)</description></item>
    /// <item><description>A SHA‑256 hash of the plaintext token (hex‑encoded)</description></item>
    /// </list>
    /// <para>
    /// The plaintext token is suitable for cookie storage, while the hashed
    /// token is used for database persistence and lookup.
    /// </para>
    /// </summary>
    /// <returns>
    /// A <see cref="GeneratedSessionToken"/> containing both the plaintext token
    /// and its hashed representation.
    /// </returns>
    public GeneratedSessionToken Generate()
    {
        // 1. Generate secure random plaintext token (256-bit)
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var plaintextToken = Convert.ToHexString(tokenBytes).ToLowerInvariant();

        // 2. Hash for DB storage
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(plaintextToken));
        var tokenHash = Convert.ToHexString(hashBytes).ToLowerInvariant();

        return new GeneratedSessionToken(
            PlaintextToken: plaintextToken,
            HashedToken: SessionTokenHash.From(tokenHash)
        );
    }

    /// <summary>
    /// Computes the SHA‑256 hash of a plaintext session token.
    /// <para>
    /// This method is used by middleware and authentication components to
    /// validate incoming session cookies by hashing the plaintext token and
    /// comparing it to the stored hash.
    /// </para>
    /// </summary>
    /// <param name="plaintextToken">
    /// The plaintext token extracted from the session cookie.
    /// </param>
    /// <returns>
    /// A <see cref="SessionTokenHash"/> representing the SHA‑256 hash of the
    /// provided plaintext token.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided token is null, empty, or whitespace.
    /// </exception>
    public SessionTokenHash Hash(string plaintextToken)
    {
        if (string.IsNullOrWhiteSpace(plaintextToken))
            throw new ArgumentException("Token cannot be null or empty.", nameof(plaintextToken));

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(plaintextToken));
        var tokenHash = Convert.ToHexString(hashBytes).ToLowerInvariant();

        return SessionTokenHash.From(tokenHash);
    }
}
