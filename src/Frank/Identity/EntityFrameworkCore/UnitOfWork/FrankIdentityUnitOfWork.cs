using Frank.Core.EntityFrameworkCore;
using Frank.Identity.Application.Abstractions.UnitOfWork;
using Frank.Identity.EntityFrameworkCore.DbContexts;

namespace Frank.Identity.EntityFrameworkCore.UnitOfWork;

/// <summary>
/// Entity Framework Core–backed implementation of <see cref="IFrankIdentityUnitOfWork"/>.
/// <para>
/// This unit of work coordinates transactional boundaries for the Identity subsystem.
/// It inherits from <see cref="EntityFrameworkCoreUnitOfWorkBase{TContext}"/>, which
/// provides standard EF Core commit and rollback behavior.
/// </para>
/// <para>
/// The unit of work ensures that multiple operations performed within a single
/// Identity vertical slice (e.g., creating a session, revoking a session, updating
/// user identity data) are committed atomically.
/// </para>
/// </summary>
public sealed class FrankIdentityUnitOfWork(FrankIdentityDbContext dbContext) :
    EntityFrameworkCoreUnitOfWorkBase<FrankIdentityDbContext>(dbContext),
    IFrankIdentityUnitOfWork
{
    // No additional behavior is required; all transactional logic is inherited.
}
