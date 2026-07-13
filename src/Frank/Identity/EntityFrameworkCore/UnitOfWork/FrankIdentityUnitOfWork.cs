using Frank.Core.Application.Abstractions.UnitOfWork;
using Frank.Identity.EntityFrameworkCore.Persistence;
using Frank.Core.EntityFrameworkCore;

namespace Frank.Identity.EntityFrameworkCore.UnitOfWork;

public sealed class FrankIdentityUnitOfWork(FrankIdentityDbContext dbContext) :
    EntityFrameworkCoreUnitOfWorkBase<FrankIdentityDbContext>(dbContext),
    IFrankIdentityUnitOfWork
{
}
