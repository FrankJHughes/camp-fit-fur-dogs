using CampFitFurDogs.Domain.Customers;
using Frank.Domain;

namespace CampFitFurDogs.Domain.Sessions;

public sealed class Session : AggregateRoot<SessionId>
{
    public SessionTokenHash TokenHash { get; }
    public CustomerId OwnerId { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? RevokedAt { get; private set; }

#pragma warning disable CS8618
    private Session() : base(default!)
    {
        // EF Core only
    }
#pragma warning restore CS8618

    private Session(
        SessionId sessionId,
        SessionTokenHash tokenHash,
        CustomerId ownerId,
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

    public static Session Create(
        SessionTokenHash tokenHash,
        CustomerId ownerId,
        DateTimeOffset createdAt)
        => new(SessionId.New(), tokenHash, ownerId, createdAt);

    // ------------------------------------------------------------
    // Domain Behavior
    // ------------------------------------------------------------

    /// <summary>
    /// Returns true if the session has expired based on the given TTL.
    /// </summary>
    public bool IsExpired(DateTimeOffset now, TimeSpan ttl)
        => CreatedAt + ttl < now;

    /// <summary>
    /// Returns true if the session is revoked.
    /// </summary>
    public bool IsRevoked()
        => RevokedAt is not null;

    /// <summary>
    /// Returns true if the session is active (not expired and not revoked).
    /// </summary>
    public bool IsActive(DateTimeOffset now, TimeSpan ttl)
        => !IsExpired(now, ttl) && !IsRevoked();

    /// <summary>
    /// Revokes the session at the given timestamp.
    /// </summary>
    public void Revoke(DateTimeOffset now)
    {
        if (RevokedAt is not null)
            throw new InvalidOperationException("Session is already revoked.");

        RevokedAt = now;
    }
}
