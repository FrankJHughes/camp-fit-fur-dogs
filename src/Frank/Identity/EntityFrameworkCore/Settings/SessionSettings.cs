namespace Frank.Identity.EntityFrameworkCore.Settings;

/// <summary>
/// Represents configuration settings for Identity session management.
/// <para>
/// These settings are bound from configuration (typically <c>Identity:Session</c>)
/// and validated at application startup. The primary setting is the session
/// time‑to‑live (TTL), which determines how long a session remains valid after
/// creation.
/// </para>
/// <para>
/// The TTL is consumed by <see cref="Frank.Identity.EntityFrameworkCore.Sessions.GetSessionReader"/>
/// to compute expiration using:
/// <c>ExpiresAt = CreatedAt + Ttl</c>.
/// </para>
/// </summary>
public sealed class SessionSettings
{
    /// <summary>
    /// Gets the configured time‑to‑live (TTL) for sessions.
    /// <para>
    /// This value defines how long a session remains valid after its creation
    /// timestamp. Once the TTL has elapsed, the session is considered expired
    /// even if it has not been explicitly revoked.
    /// </para>
    /// </summary>
    public TimeSpan Ttl { get; init; }
}
