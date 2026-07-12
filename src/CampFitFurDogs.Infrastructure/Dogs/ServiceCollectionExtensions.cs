using CampFitFurDogs.Application.Abstractions.Dog.GetDogProfile;
using CampFitFurDogs.Application.Abstractions.Dog.ListDogsByOwner;
using CampFitFurDogs.Domain.Dogs;
using Microsoft.Extensions.DependencyInjection;

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
