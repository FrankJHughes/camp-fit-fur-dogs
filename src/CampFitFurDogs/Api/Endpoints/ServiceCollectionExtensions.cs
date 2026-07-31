
using CampFitFurDogs.Api.Endpoints.Dogs;
using CampFitFurDogs.Api.Endpoints.Health;

namespace CampFitFurDogs.Api.Endpoints;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCampFitFurDogsApiEndpoints(
        this IServiceCollection services)
    {
        return services

            .AddCampFitFurDogsApiEndpointsDogs()
            .AddCampFitFurDogsApiEndpointsHealth()
            ;
    }
}
