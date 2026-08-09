using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Abstractions.Sessions.CreateSession;

/// <summary>
/// Defines the contract for persisting a newly created <see cref="Session"/>
/// into the underlying session store.
/// <para>
/// The session‑creation pipeline produces a fully constructed domain
/// <see cref="Session"/> object containing all required metadata such as:
/// </para>
/// <list type="bullet">
/// <item><description>User identifier</description></item>
/// <item><description>Session identifier</description></item>
/// <item><description>Token hash</description></item>
/// <item><description>Creation timestamp</description></item>
/// <item><description>Expiration rules</description></item>
/// </list>
/// <para>
/// Implementations of <see cref="ICreateSessionWriter"/> are responsible for
/// writing this domain object to persistent storage (e.g., database, cache,
/// distributed session store) in a durable and atomic manner.
/// </para>
/// </summary>
/// <remarks>
/// This abstraction separates domain‑level session creation from infrastructure
/// concerns, enabling:
/// <list type="bullet">
/// <item><description>Testability</description></item>
/// <item><description>Clear boundaries between domain and persistence</description></item>
/// <item><description>Multiple storage implementations (SQL, NoSQL, Redis, etc.)</description></item>
/// <item><description>Deterministic session‑creation pipelines</description></item>
/// </list>
/// </remarks>
public interface ICreateSessionWriter
{
    /// <summary>
    /// Persists the specified <see cref="Session"/> into the session store.
    /// <para>
    /// Implementations must ensure that the session is written durably and
    /// consistently, and may apply additional infrastructure‑level behaviors such
    /// as transactional guarantees, concurrency control, or replication.
    /// </para>
    /// <para>
    /// The method is asynchronous and supports cancellation via
    /// <paramref name="ct"/>.
    /// </para>
    /// </summary>
    /// <param name="session">
    /// The fully constructed domain <see cref="Session"/> to persist.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that allows the caller to cancel the write operation.
    /// </param>
    Task WriteAsync(Session session, CancellationToken ct);
}
