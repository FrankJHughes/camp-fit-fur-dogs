using Frank.Core.EntityFrameworkCore;
using Frank.Identity.Application.Abstractions.UnitOfWork;
using Frank.Identity.EntityFrameworkCore.DbContexts;

namespace Frank.Identity.EntityFrameworkCore.UnitOfWork;

public sealed class FrankIdentityUnitOfWork(FrankIdentityDbContext dbContext) :
    EntityFrameworkCoreUnitOfWorkBase<FrankIdentityDbContext>(dbContext),
    IFrankIdentityUnitOfWork
{
}
