using System.Reflection;
using FluentValidation;

namespace CampFitFurDogs.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(
        this IServiceCollection services)
    {
        var assemblies = new Assembly[]
        {
            typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly
        };

        services.AddValidatorsFromAssemblies(assemblies);

        return services;
    }
}
