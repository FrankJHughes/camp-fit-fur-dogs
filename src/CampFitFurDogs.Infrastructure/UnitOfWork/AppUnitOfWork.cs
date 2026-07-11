using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Infrastructure.Persistence;

namespace Frank.Infrastructure.EntityFrameworkCore;

public sealed class AppUnitOfWork(AppDbContext dbContext) :
    EntityFrameworkCoreUnitOfWorkBase<AppDbContext>(dbContext),
    IAppUnitOfWork
{
}
