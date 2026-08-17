using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Abstractions.Sessions;

/// <summary>
/// Represents the pair of values produced when generating a new session token.
/// <para>
/// The Identity subsystem generates a secure, random plaintext session token for
/// the client to store (typically in a cookie).
/// A corresponding <see cref="SessionTokenHash"/> is computed and persisted in
/// durable storage.
/// </para>
/// <para>
/// The plaintext token is **never persisted** and is only returned once at
/// creation time.
/// The hashed token is used for all future lookups, revocation, and validation.
/// </para>
/// </summary>
/// <param name="PlaintextToken">
/// The raw, randomly generated session token intended for client storage.
/// This value must never be logged or persisted.
/// </param>
/// <param name="HashedToken">
/// The secure, non‑reversible hash of the session token.
/// This value is persisted and used to identify the session in storage.
/// </param>
public sealed record GeneratedSessionToken(
    string PlaintextToken,
    SessionTokenHash HashedToken
);
