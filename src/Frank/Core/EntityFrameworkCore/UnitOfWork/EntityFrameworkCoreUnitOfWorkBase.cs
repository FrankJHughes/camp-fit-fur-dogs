using Frank.Core.Application.Abstractions.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Frank.Core.EntityFrameworkCore;

/// <summary>
/// Provides a base implementation of the Unit of Work pattern for Entity
/// Framework Core <see cref="DbContext"/> instances.
///
/// <para>
/// This class encapsulates the commit boundary for persistence operations,
/// ensuring that all tracked changes within the underlying <typeparamref name="TContext"/>
/// are saved atomically. It serves as the application‑layer abstraction over
/// EF Core’s change tracking and database write operations.
/// </para>
///
/// <para>
/// Derived implementations may extend this class to add behaviors such as:
/// <list type="bullet">
///   <item><description>pre‑commit validation</description></item>
///   <item><description>domain event dispatching</description></item>
///   <item><description>audit logging</description></item>
///   <item><description>transaction management</description></item>
/// </list>
/// </para>
/// </summary>
/// <typeparam name="TContext">
/// The <see cref="DbContext"/> type used for persistence.
/// </typeparam>
public class EntityFrameworkCoreUnitOfWorkBase<TContext> : IUnitOfWork
    where TContext : DbContext
{
    private readonly TContext _dbContext = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFrameworkCoreUnitOfWorkBase{TContext}"/>
    /// class using the provided <see cref="DbContext"/>.
    /// </summary>
    /// <param name="dbContext">
    /// The EF Core context responsible for tracking and persisting changes.
    /// </param>
    public EntityFrameworkCoreUnitOfWorkBase(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Commits all tracked changes to the database by invoking
    /// <see cref="DbContext.SaveChangesAsync(CancellationToken)"/>.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        // 1. Save changes
        var result = await _dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }
}
