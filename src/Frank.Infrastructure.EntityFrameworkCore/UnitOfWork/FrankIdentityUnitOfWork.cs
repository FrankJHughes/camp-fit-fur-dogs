using Frank.Abstractions.UnitOfWork;
using Frank.Infrastructure.EntityFrameworkCore.Persistence;

namespace Frank.Infrastructure.EntityFrameworkCore.UnitOfWork;

public sealed class FrankIdentityUnitOfWork(FrankIdentityDbContext dbContext) :
    EntityFrameworkCoreUnitOfWorkBase<FrankIdentityDbContext>(dbContext),
    IFrankIdentityUnitOfWork
{
}
