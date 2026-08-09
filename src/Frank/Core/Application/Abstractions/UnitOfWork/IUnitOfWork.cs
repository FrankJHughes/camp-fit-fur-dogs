namespace Frank.Core.Application.Abstractions.UnitOfWork;

/// <summary>
/// Represents a transactional boundary for committing changes to the underlying
/// persistence mechanism.
///
/// <para>
/// A unit of work coordinates the writing of changes across multiple repositories
/// or data sources, ensuring that all operations either succeed together or fail
/// together. Implementations typically wrap a database transaction or equivalent
/// atomic operation.
/// </para>
///
/// <para>
/// The unit of work pattern helps maintain consistency, prevents partial writes,
/// and provides a clear commit point for application workflows.
/// </para>
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Commits all pending changes within the current unit of work.
    ///
    /// <para>
    /// Implementations should ensure atomicity: either all changes are persisted,
    /// or none are. The returned integer typically represents the number of
    /// affected records, though exact semantics may vary by implementation.
    /// </para>
    /// </summary>
    /// <param name="ct">
    /// An optional cancellation token used to cancel the commit operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous commit operation, containing the
    /// number of affected records.
    /// </returns>
    Task<int> CommitAsync(CancellationToken ct = default);
}
