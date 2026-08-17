using Frank.Core.Domain;
using Frank.Identity.Domain.Users;

namespace Frank.Identity.Domain.Sessions;

/// <summary>
/// Represents an authenticated session within the Identity domain.
/// <para>
/// A session is created when an owner (user) successfully authenticates and
/// receives a session token. The token is stored only as a
/// <see cref="SessionTokenHash"/> for security reasons.
/// </para>
/// <para>
/// The session tracks creation time, revocation time, and supports domain
/// behaviors such as expiration and revocation.
/// </para>
/// </summary>
public sealed class Session : AggregateRoot<SessionId>
{
    /// <summary>
    /// Gets the SHA‑256 hash of the session token associated with this session.
    /// <para>
    /// The raw token is never stored; only its hash is persisted to ensure
    /// secure lookup and prevent token leakage.
    /// </para>
    /// </summary>
    public SessionTokenHash TokenHash { get; }

    /// <summary>
    /// Gets the identifier of the user who owns this session.
    /// </summary>
    public UserId OwnerId { get; }

    /// <summary>
    /// Gets the timestamp at which the session was created.
    /// <para>
    /// This value is used to determine expiration based on configured TTL.
    /// </para>
    /// </summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the timestamp at which the session was revoked, if any.
    /// <para>
    /// A revoked session is permanently inactive regardless of expiration.
    /// </para>
    /// </summary>
    public DateTimeOffset? RevokedAt { get; private set; }

#pragma warning disable CS8618
    /// <summary>
    /// EF Core constructor. Not intended for direct use.
    /// </summary>
    private Session() : base(default!)
    {
        // EF Core only
    }
#pragma warning restore CS8618

    /// <summary>
    /// Initializes a new <see cref="Session"/> with the specified values.
    /// </summary>
    /// <param name="sessionId">The unique session identifier.</param>
    /// <param name="tokenHash">The SHA‑256 hash of the session token.</param>
    /// <param name="ownerId">The identifier of the user who owns the session.</param>
    /// <param name="createdAt">The timestamp at which the session was created.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="tokenHash"/> or <paramref name="ownerId"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="createdAt"/> is the default timestamp.
    /// </exception>
    private Session(
        SessionId sessionId,
        SessionTokenHash tokenHash,
        UserId ownerId,
        DateTimeOffset createdAt)
        : base(sessionId)
    {
        ArgumentNullException.ThrowIfNull(tokenHash);
        ArgumentNullException.ThrowIfNull(ownerId);

        if (createdAt == default)
            throw new ArgumentException("CreatedAt must be a valid timestamp.", nameof(createdAt));

        TokenHash = tokenHash;
        OwnerId = ownerId;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates a new <see cref="Session"/> using a freshly generated
    /// <see cref="SessionId"/> and the provided token hash, owner ID, and creation timestamp.
    /// </summary>
    /// <param name="tokenHash">The SHA‑256 hash of the session token.</param>
    /// <param name="ownerId">The identifier of the user who owns the session.</param>
    /// <param name="createdAt">The timestamp at which the session was created.</param>
    /// <returns>A new <see cref="Session"/> instance.</returns>
    public static Session Create(
        SessionTokenHash tokenHash,
        UserId ownerId,
        DateTimeOffset createdAt)
        => new(SessionId.New(), tokenHash, ownerId, createdAt);

    // ------------------------------------------------------------
    // Domain Behavior
    // ------------------------------------------------------------

    /// <summary>
    /// Determines whether the session has expired based on the provided TTL.
    /// </summary>
    /// <param name="now">The current timestamp.</param>
    /// <param name="ttl">The time-to-live duration for sessions.</param>
    /// <returns>
    /// <c>true</c> if the session has expired; otherwise <c>false</c>.
    /// </returns>
    public bool IsExpired(DateTimeOffset now, TimeSpan ttl)
        => CreatedAt + ttl < now;

    /// <summary>
    /// Determines whether the session has been revoked.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the session is revoked; otherwise <c>false</c>.
    /// </returns>
    public bool IsRevoked()
        => RevokedAt is not null;

    /// <summary>
    /// Determines whether the session is active.
    /// <para>
    /// A session is active if it is neither expired nor revoked.
    /// </para>
    /// </summary>
    /// <param name="now">The current timestamp.</param>
    /// <param name="ttl">The time-to-live duration for sessions.</param>
    /// <returns>
    /// <c>true</c> if the session is active; otherwise <c>false</c>.
    /// </returns>
    public bool IsActive(DateTimeOffset now, TimeSpan ttl)
        => !IsExpired(now, ttl) && !IsRevoked();

    /// <summary>
    /// Revokes the session at the specified timestamp.
    /// <para>
    /// Once revoked, a session cannot be reactivated.
    /// </para>
    /// </summary>
    /// <param name="now">The timestamp at which the session is revoked.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the session is already revoked.
    /// </exception>
    public void Revoke(DateTimeOffset now)
    {
        if (RevokedAt is not null)
            throw new InvalidOperationException("Session is already revoked.");

        RevokedAt = now;
    }
}
