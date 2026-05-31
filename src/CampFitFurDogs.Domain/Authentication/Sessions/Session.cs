using CampFitFurDogs.Domain.Customers;
using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Authentication.Sessions;

public sealed class Session : AggregateRoot<SessionId>
{
    public SessionTokenHash TokenHash { get; }
    public CustomerId OwnerId { get; }
    public DateTimeOffset CreatedAt { get; }

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
        // -----------------------------
        // Domain invariants
        // -----------------------------
        if (tokenHash is null)
            throw new ArgumentNullException(nameof(tokenHash));

        if (ownerId is null)
            throw new ArgumentNullException(nameof(ownerId));

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
}
