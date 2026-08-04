using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogById;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Infrastructure.Dogs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureDogs(this IServiceCollection services)
    {
        return services
            .AddScoped<IEditDogWriter, EditDogWriter>()
            .AddScoped<IRegisterDogWriter, RegisterDogWriter>()
            .AddScoped<IRemoveDogWriter, RemoveDogWriter>()
            .AddScoped<IGetDogByIdReader, GetDogByIdReader>()
            .AddScoped<IGetDogReader, GetDogReader>()
            .AddScoped<IListDogsByOwnerReader, ListDogsByOwnerReader>();
    }
}
