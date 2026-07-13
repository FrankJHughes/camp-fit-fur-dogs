namespace Frank.Identity.Application.Abstractions.Sessions.GetSession;

public sealed record GetSessionResponse(
    Guid Id,
    Guid OwnerId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? RevokedAt,
    DateTimeOffset ExpiresAt)
{
    public bool IsExpired => ExpiresAt <= DateTimeOffset.UtcNow;

    public bool IsRevoked => RevokedAt is not null;

    public bool IsActive => !IsExpired && !IsRevoked;
}
