using CampFitFurDogs.Infrastructure.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using Frank.Application.Abstractions.Audit;
using Frank.Infrastructure.Audit;
using Frank.Infrastructure.EntityFrameworkCore.Persistence;
using Frank.Infrastructure.EntityFrameworkCore.Sessions;
using Frank.Infrastructure.EntityFrameworkCore.UnitOfWork;
using Frank.Infrastructure.EntityFrameworkCore.Users;
using Frank.Infrastructure.Environment;
using Frank.Infrastructure.Identity;
using Frank.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
