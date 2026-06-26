using Microsoft.Extensions.DependencyInjection;

using Frank.Authentication.Callback;
using Frank.Command;
using Frank.Query;

using CampFitFurDogs.Application.Authentication.Callback;
using CampFitFurDogs.Application.Settings;
using Frank.Event;

namespace CampFitFurDogs.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddFrankAuthCallback(); // IImmutableContextBuilder<FrankAuthCallbackRequest, OidcAuthCallbackContext, FrankAuthCallbackResult>

        services
            .AddOptions<AuthCallbackSettings>()
            .BindConfiguration("Frontend")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddApplicationAuthCallback();

        services.AddFrankCommand();
        services.AddFrankQuery();
        services.AddFrankEvent();

        services
            .AddOptions<FrontendSettings>()
            .BindConfiguration("Frontend")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
