using System.Security.Cryptography;
using System.Text;
using Frank.Identity.Application.Abstractions.Sessions;
using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application;

public sealed class SessionTokenService : ISessionTokenService
{
    /// <summary>
    /// Generates a new secure session token:
    /// - Plaintext token (256-bit, hex-encoded) for the cookie
    /// - SHA-256 hash (hex-encoded) for DB storage
    /// </summary>
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
    /// Hashes a plaintext session token using SHA-256.
    /// Used by middleware to validate incoming cookies.
    /// </summary>
    public SessionTokenHash Hash(string plaintextToken)
    {
        if (string.IsNullOrWhiteSpace(plaintextToken))
            throw new ArgumentException("Token cannot be null or empty.", nameof(plaintextToken));

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(plaintextToken));
        var tokenHash = Convert.ToHexString(hashBytes).ToLowerInvariant();

        return SessionTokenHash.From(tokenHash);
    }
}
