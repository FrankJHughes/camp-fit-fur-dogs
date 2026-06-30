using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Infrastructure.Audit;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Infrastructure.Identity;
using Frank.Infrastructure.EntityFrameworkCore.UnitOfWork;
using Frank.Infrastructure.Environment;
using CampFitFurDogs.Infrastructure.Customers;
using CampFitFurDogs.Infrastructure.Dogs;
using Frank.Infrastructure.Time;
using CampFitFurDogs.Infrastructure.Sessions;

namespace CampFitFurDogs.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        return services
            .AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                })

            .AddHttpContextAccessor()

            .AddFrankEnvironment() // IEnvironment
            .AddFrankTime() // IClock
            .AddFrankEntityFrameworkCoreUnitOfWork<AppDbContext>() // IUnitOfWork

            .AddCustomerInfrastructure()
            .AddDogInfrastructure()
            .AddIdentityInfrastructure()
            .AddSessionInfrastructure()

            .AddSingleton<IAuditLogger, AuditLogger>();

    }
}
