using Frank.Core.Application.Abstractions.UnitOfWork;
using Frank.Core.EntityFrameworkCore;
using Frank.Identity.EntityFrameworkCore.DbContexts;

namespace Frank.Identity.EntityFrameworkCore.UnitOfWork;

public sealed class FrankIdentityUnitOfWork(FrankIdentityDbContext dbContext) :
    EntityFrameworkCoreUnitOfWorkBase<FrankIdentityDbContext>(dbContext),
    IFrankIdentityUnitOfWork
{
}
