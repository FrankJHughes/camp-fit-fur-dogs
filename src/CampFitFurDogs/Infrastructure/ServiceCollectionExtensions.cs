using CampFitFurDogs.Infrastructure.DbContexts;
using CampFitFurDogs.Infrastructure.Dogs;
using Frank.Identity.EntityFrameworkCore.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCampFitFurDogsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        return services
            .AddHttpContextAccessor()

            .AddInfrastructureDbContexts(configuration)
            .AddInfrastructureDogs()
            .AddInfrastructureUnitOfWork()
            ;


    }
}
