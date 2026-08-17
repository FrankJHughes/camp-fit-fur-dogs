using CampFitFurDogs.Application.Abstractions.UnitOfWork;
using CampFitFurDogs.Infrastructure.Persistence;
using Frank.Core.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.UnitOfWork;

/// <summary>
/// Represents the application’s Entity Framework Core–backed unit of work.
/// <para>
/// This implementation wraps <see cref="AppDbContext"/> and provides a
/// consistent transactional boundary for application‑layer workflows.
/// </para>
/// <para>
/// The class derives from
/// <see cref="EntityFrameworkCoreUnitOfWorkBase{TContext}"/>, which supplies
/// standard commit and rollback behavior, ensuring that all changes tracked by
/// <see cref="AppDbContext"/> are persisted atomically.
/// </para>
/// </summary>
public sealed class AppUnitOfWork :
    EntityFrameworkCoreUnitOfWorkBase<AppDbContext>,
    IAppUnitOfWork
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppUnitOfWork"/> class.
    /// </summary>
    /// <param name="dbContext">
    /// The <see cref="AppDbContext"/> instance used to track and commit changes.
    /// </param>
    public AppUnitOfWork(AppDbContext dbContext)
        : base(dbContext)
    {
    }
}
