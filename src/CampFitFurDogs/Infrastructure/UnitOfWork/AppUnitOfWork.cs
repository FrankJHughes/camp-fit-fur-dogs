using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Infrastructure.Persistence;

namespace Frank.Core.EntityFrameworkCore;

public sealed class AppUnitOfWork(AppDbContext dbContext) :
    EntityFrameworkCoreUnitOfWorkBase<AppDbContext>(dbContext),
    IAppUnitOfWork
{
}
