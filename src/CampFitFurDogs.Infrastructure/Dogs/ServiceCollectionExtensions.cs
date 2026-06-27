using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

namespace CampFitFurDogs.Infrastructure.Dogs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDogInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<IDogRepository, DogRepository>()
            .AddScoped<IGetDogProfileReader, GetDogProfileReader>()
            .AddScoped<IListDogsByOwnerReader, ListDogsByOwnerReader>();
    }
}
