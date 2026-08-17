using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Abstractions.Sessions.RevokeSession;

/// <summary>
/// Defines the contract for revoking an existing session based on its token hash.
/// <para>
/// This writer is part of the Identity subsystem’s session‑revocation pipeline.
/// It is responsible for marking a persisted session as revoked, ensuring that
/// subsequent authentication attempts using the associated token are rejected.
/// </para>
/// <para>
/// Only the secure, non‑reversible <see cref="SessionTokenHash"/> is used to
/// identify the session. The raw token is never persisted.
/// </para>
/// </summary>
/// <remarks>
/// Implementations may apply infrastructure‑level behaviors such as:
/// <list type="bullet">
/// <item><description>Database or cache updates</description></item>
/// <item><description>Atomic write guarantees</description></item>
/// <item><description>Concurrency control</description></item>
/// <item><description>Replication or distributed invalidation</description></item>
/// </list>
/// The writer does not evaluate expiration or active status; those rules are
/// handled by the session‑retrieval pipeline and <c>GetSessionResponse</c>.
/// </remarks>
public interface IRevokeSessionWriter
{
    /// <summary>
    /// Revokes the session associated with the specified token hash.
    /// <para>
    /// Implementations must ensure that the session is marked as revoked in a
    /// durable and consistent manner. If no session exists for the provided hash,
    /// implementations may choose to ignore the request or log the event.
    /// </para>
    /// <para>
    /// The method is asynchronous and supports cancellation via
    /// <paramref name="ct"/>.
    /// </para>
    /// </summary>
    /// <param name="tokenHash">
    /// The secure, non‑reversible hash of the session token identifying the
    /// session to revoke.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that allows the caller to cancel the revoke
    /// operation.
    /// </param>
    Task WriteAsync(SessionTokenHash tokenHash, CancellationToken ct);
}
