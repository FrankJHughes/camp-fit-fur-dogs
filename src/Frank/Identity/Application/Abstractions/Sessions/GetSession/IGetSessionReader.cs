namespace Frank.Identity.Application.Abstractions.Sessions.GetSession;

/// <summary>
/// Defines the contract for retrieving a session based on its token hash.
/// <para>
/// This reader is part of the Identity subsystem’s session‑retrieval pipeline.
/// It is responsible for looking up a persisted session using the secure,
/// non‑reversible token hash presented by the client (typically via a cookie or
/// header).
/// </para>
/// <para>
/// Only the token hash is stored in persistent storage; the raw token is never
/// persisted.
/// The reader returns a <see cref="GetSessionResponse"/> when a matching session
/// exists, or <c>null</c> when no session is found.
/// </para>
/// </summary>
/// <remarks>
/// Implementations may apply additional infrastructure‑level behaviors such as:
/// <list type="bullet">
/// <item><description>Database or cache lookup</description></item>
/// <item><description>Expiration checks</description></item>
/// <item><description>Revocation checks</description></item>
/// <item><description>Consistency or transactional guarantees</description></item>
/// </list>
/// The reader itself does not evaluate expiration or revocation; those rules are
/// handled by <see cref="GetSessionResponse"/> using the <c>EvaluatedAt</c>
/// timestamp supplied by the query handler.
/// </remarks>
public interface IGetSessionReader
{
    /// <summary>
    /// Retrieves a session using its token hash.
    /// <para>
    /// The token hash uniquely identifies the session in persistent storage.
    /// Implementations must ensure secure lookup and return <c>null</c> when no
    /// matching session exists.
    /// </para>
    /// <para>
    /// The method is asynchronous and supports cancellation via
    /// <paramref name="ct"/>.
    /// </para>
    /// </summary>
    /// <param name="tokenHash">
    /// The secure, non‑reversible hash of the session token presented by the
    /// client.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that allows the caller to cancel the read operation.
    /// </param>
    /// <returns>
    /// A <see cref="GetSessionResponse"/> representing the resolved session, or
    /// <c>null</c> if no session is found.
    /// </returns>
    Task<GetSessionResponse?> ReadAsync(string tokenHash, CancellationToken ct);
}
