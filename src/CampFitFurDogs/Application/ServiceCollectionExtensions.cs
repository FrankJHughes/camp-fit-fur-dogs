using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.DomainEvents;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Callback.Save;
using Frank.Core.Application.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(
        this IServiceCollection services)
    {
        // Frank.Identity
        services.AddFrankCqrsCommands([
            typeof(Frank.Identity.Application.AssemblyMarker).Assembly
        ]);
        services.AddFrankCqrsQueries([
            typeof(Frank.Identity.Application.AssemblyMarker).Assembly
        ]);
        services.AddFrankIdentityCallbackOidc(); // authenticate
        services.AddFrankIdentityCallbackSave(); // resolve user and session

        // CampFitFurDogs CQRS
        services.AddFrankCqrsCommands([
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly
        ]);
        services.AddFrankCqrsQueries([
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly
        ]);

        services.AddFrankDomainEvents();

        return services;
    }
}
