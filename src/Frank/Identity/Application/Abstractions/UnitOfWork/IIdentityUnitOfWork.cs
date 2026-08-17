using Frank.Core.Application.Abstractions.UnitOfWork;

namespace Frank.Identity.Application.Abstractions.UnitOfWork;

/// <summary>
/// Represents the Identity subsystem’s specialized unit of work abstraction.
/// <para>
/// This interface extends the core <see cref="IUnitOfWork"/> contract and serves
/// as the boundary for transactional operations within the Identity domain.
/// </para>
/// <para>
/// Identity operations—such as session creation, session revocation, and
/// owner‑related state changes—may require transactional guarantees depending on
/// the underlying persistence mechanism.
/// This interface allows the application layer to depend on a stable,
/// Identity‑specific unit of work without referencing infrastructure concerns.
/// </para>
/// </summary>
/// <remarks>
/// Infrastructure implementations may wrap:
/// <list type="bullet">
/// <item><description>Database transactions</description></item>
/// <item><description>Distributed transaction scopes</description></item>
/// <item><description>Atomic write batches (e.g., in NoSQL stores)</description></item>
/// <item><description>Event‑sourced commit boundaries</description></item>
/// </list>
/// The application layer never assumes the underlying mechanism; it only invokes
/// <see cref="IUnitOfWork.CommitAsync"/> or <see cref="IUnitOfWork.RollbackAsync"/>
/// through this abstraction.
/// </remarks>
public interface IFrankIdentityUnitOfWork : IUnitOfWork { }
