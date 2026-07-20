using CampFitFurDogs.Infrastructure.DbContexts;
using CampFitFurDogs.Infrastructure.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using Frank.Core.Application.Abstractions.Audit;
using Frank.Core.Infrastructure.Audit;
using Frank.Core.Infrastructure.EnvironmentVariables;
using Frank.Core.Infrastructure.Time;
using Frank.Identity.EntityFrameworkCore;
using Frank.Identity.EntityFrameworkCore.UnitOfWork;
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

            .AddFrankIdentityEntityFrmeworkCore(configuration)

            .AddFrankEnvironment()
            .AddFrankTime()


            .AddSingleton<IAuditLogger, AuditLogger>()

            .AddInfrastructureDbContexts(configuration)

            .AddInfrastructureUnitOfWork()

            .AddInfrastructureDogs();


    }
}
