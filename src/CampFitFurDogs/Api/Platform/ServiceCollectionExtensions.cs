using CampFitFurDogs.Api.ExceptionHandlers;
using CampFitFurDogs.Application;
using CampFitFurDogs.Infrastructure;

namespace CampFitFurDogs.Api.Platform;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCampFitFurDogsApiPlatform(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddCampFitFurDogsApplication()
            .AddCampFitFurDogsInfrastructure(configuration)
            .AddCampFitFurDogsApiExceptionHandlers()
            ;

    }
}
