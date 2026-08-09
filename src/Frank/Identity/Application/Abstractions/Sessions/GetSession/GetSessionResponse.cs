namespace Frank.Identity.Application.Abstractions.Sessions.GetSession;

/// <summary>
/// Represents the application‑level response returned when a session is resolved
/// by its token hash.
/// <para>
/// This model is used by the session‑retrieval pipeline to expose the essential
/// attributes of a domain <c>Session</c> in a safe, immutable format suitable for
/// authentication and authorization flows.
/// </para>
/// <para>
/// The response includes timestamps, revocation status, expiration rules, and
/// convenience properties for determining whether the session is currently
/// active.
/// All evaluations are performed relative to <see cref="EvaluatedAt"/>, which is
/// captured by the query handler using the injected <see cref="IClock"/>.
/// </para>
/// </summary>
/// <param name="Id">
/// The unique identifier of the session.
/// This value corresponds to the domain session’s primary key.
/// </param>
/// <param name="OwnerId">
/// The identifier of the user (owner) associated with the session.
/// </param>
/// <param name="CreatedAt">
/// The timestamp indicating when the session was created.
/// </param>
/// <param name="RevokedAt">
/// The timestamp indicating when the session was revoked, if applicable.
/// A <c>null</c> value means the session has not been revoked.
/// </param>
/// <param name="ExpiresAt">
/// The timestamp indicating when the session expires.
/// After this time, the session is considered invalid regardless of revocation.
/// </param>
/// <param name="EvaluatedAt">
/// The timestamp at which the session’s status was evaluated.
/// This value is captured by the query handler using the <see cref="IClock"/>
/// abstraction, ensuring deterministic and replayable evaluation.
/// </param>
public sealed record GetSessionResponse(
    Guid Id,
    Guid OwnerId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? RevokedAt,
    DateTimeOffset ExpiresAt,
    DateTimeOffset EvaluatedAt)
{
    /// <summary>
    /// Indicates whether the session has expired relative to
    /// <see cref="EvaluatedAt"/>.
    /// <para>
    /// A session is expired when <see cref="ExpiresAt"/> is less than or equal to
    /// <see cref="EvaluatedAt"/>.
    /// </para>
    /// </summary>
    public bool IsExpired => ExpiresAt <= EvaluatedAt;

    /// <summary>
    /// Indicates whether the session has been explicitly revoked.
    /// <para>
    /// A session is revoked when <see cref="RevokedAt"/> contains a timestamp.
    /// </para>
    /// </summary>
    public bool IsRevoked => RevokedAt is not null;

    /// <summary>
    /// Indicates whether the session is currently active relative to
    /// <see cref="EvaluatedAt"/>.
    /// <para>
    /// A session is active only when it is neither expired nor revoked.
    /// </para>
    /// </summary>
    public bool IsActive => !IsExpired && !IsRevoked;
}
