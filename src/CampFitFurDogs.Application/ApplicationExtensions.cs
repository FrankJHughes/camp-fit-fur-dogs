using CampFitFurDogs.Application.Authentication.Callback;
using CampFitFurDogs.Application.Settings;
using Frank.Authentication.Callback;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddFrankAuthCallback();

        services
            .AddOptions<AuthCallbackSettings>()
            .BindConfiguration("Frontend")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddApplicationAuthCallback();

        services
            .AddOptions<FrontendSettings>()
            .BindConfiguration("Frontend")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
