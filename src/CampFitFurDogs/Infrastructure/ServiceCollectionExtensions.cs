using CampFitFurDogs.Infrastructure.DbContexts;
using CampFitFurDogs.Infrastructure.Dogs;
using Frank.Core.Infrastructure.AuditLogging;
using Frank.Core.Infrastructure.EnvironmentVariables;
using Frank.Core.Infrastructure.Clock;
using Frank.Identity.EntityFrameworkCore;
using Frank.Identity.EntityFrameworkCore.UnitOfWork;
using Frank.Identity.Infrastructure;
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

            .AddFrankIdentityEntityFrmeworkCore(configuration)

            .AddFrankIdentityInfrastructure()

            .AddFrankCoreInfrastructureEnvironmentVariables()

            .AddFrankCoreInfrastructureClock()

            .AddFrankCoreInfrastructureAuditLogging()

            .AddInfrastructureDbContexts(configuration)

            .AddInfrastructureUnitOfWork()

            .AddInfrastructureDogs();


    }
}
