using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogById;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Infrastructure.Dogs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureDogs(this IServiceCollection services)
    {
        return services
            .AddScoped<IRegisterDogWriter, RegisterDogWriter>()
            .AddScoped<IRemoveDogWriter, RemoveDogWriter>()
            .AddScoped<IGetDogByIdReader, GetDogByIdReader>()
            .AddScoped<IGetDogProfileReader, GetDogProfileReader>()
            .AddScoped<IListDogsByOwnerReader, ListDogsByOwnerReader>();
    }
}
