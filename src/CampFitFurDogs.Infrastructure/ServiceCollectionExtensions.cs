using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Frank.Application.Abstractions.Audit;
using Frank.Infrastructure.Audit;
using Frank.Infrastructure.EntityFrameworkCore.Persistence;
using Frank.Infrastructure.Identity;
using Frank.Infrastructure.EntityFrameworkCore.UnitOfWork;
using Frank.Infrastructure.Environment;
using Frank.Infrastructure.EntityFrameworkCore.Users;
using CampFitFurDogs.Infrastructure.Dogs;
using Frank.Infrastructure.Time;
using Frank.Infrastructure.EntityFrameworkCore.Sessions;
using CampFitFurDogs.Infrastructure.Persistence;

namespace CampFitFurDogs.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        return services
            .AddHttpContextAccessor()

            .AddDbContext<FrankIdentityDbContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                })
            .AddFrankIdentityUnitOfWork() // IFrankIdentityUnitOfWork

            .AddFrankEnvironment() // IEnvironment
            .AddFrankTime() // IClock

            .AddFrankUsersInfrastructure()
            .AddFrankSessionsInfrastructure()
            .AddFrankIdentity()

            .AddSingleton<IAuditLogger, AuditLogger>()

            .AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                })
            .AddAppUnitOfWork() // IAppUnitOfWork

            .AddDogInfrastructure();


    }
}
