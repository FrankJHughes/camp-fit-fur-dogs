using Frank.Core.Application.Command;
using Frank.Core.Application.DomainEvents;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Callback.Save;
using Frank.Core.Application.Query;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddOidcCallback(); // IImmutableContextBuilder<OidcCallbackRequest, OidcAuthCallbackContext, OidcCallbackResult>

        services.AddSaveCallback();

        services.AddFrankCommands([
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
            typeof(Frank.Identity.Application.AssemblyMarker).Assembly // user commands
        ]);

        services.AddFrankQuery([
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
            typeof(Frank.Identity.Application.AssemblyMarker).Assembly // user queries
        ]);

        services.AddFrankDomainEvents(); // none implemented yet

        return services;
    }
}
