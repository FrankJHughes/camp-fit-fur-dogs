using Microsoft.Extensions.DependencyInjection;

using Frank.Authentication.Callback;
using Frank.Command;
using Frank.Query;

using Frank.Application.Identity.Callback;
using Frank.Event;

namespace CampFitFurDogs.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddFrankAuthCallback(); // IImmutableContextBuilder<FrankAuthCallbackRequest, OidcAuthCallbackContext, FrankAuthCallbackResult>

        services.AddApplicationAuthCallback();

        services.AddFrankCommand([
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
            typeof(Frank.AssemblyMarker).Assembly // user commands
        ]);

        services.AddFrankQuery([
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
            typeof(Frank.AssemblyMarker).Assembly // user queries
        ]);

        services.AddFrankEvent(); // none implemented yet

        return services;
    }
}
