using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Application.Abstractions.Dog.GetDogProfile;
using CampFitFurDogs.Application.Abstractions.Dog.ListDogsByOwner;

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
