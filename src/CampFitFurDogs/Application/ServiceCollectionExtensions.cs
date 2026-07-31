using CampFitFurDogs.Application.Dogs;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCampFitFurDogsApplication(
        this IServiceCollection services)
    {

        services
            .AddApplicationDogs()
            .AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly)
            ;

        return services;
    }
}


