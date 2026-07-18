using CampFitFurDogs.Infrastructure.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using Frank.Core.Application.Abstractions.Audit;
using Frank.Core.Infrastructure.Audit;
using Frank.Core.Infrastructure.EnvironmentVariables;
using Frank.Identity.Infrastructure;
using Frank.Core.Infrastructure.Time;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Frank.Identity.EntityFrameworkCore.Sessions;
using Frank.Identity.EntityFrameworkCore.UnitOfWork;
using Frank.Identity.EntityFrameworkCore.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Frank.Identity.EntityFrameworkCore;

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

            .AddFrankIdentityInfrastructure(configuration)
            .AddFrankIdentity()

            .AddFrankEnvironment() // IEnvironment
            .AddFrankTime() // IClock


            .AddSingleton<IAuditLogger, AuditLogger>()

            .AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                })
            .AddAppUnitOfWork() // IAppUnitOfWork

            .AddDogInfrastructure();


    }
}
