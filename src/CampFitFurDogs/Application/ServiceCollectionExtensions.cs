using CampFitFurDogs.Application.Dogs;
using Frank.Core.Application.DomainEvents;
using Frank.Identity.Application;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(
        this IServiceCollection services)
    {

        services

            .AddFrankIdentityApplication()

            .AddFrankDomainEvents()

            .AddApplicationDogs();

        return services;
    }
}


