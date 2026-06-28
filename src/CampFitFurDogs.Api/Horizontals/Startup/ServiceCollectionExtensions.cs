using Frank.Abstractions.Startup;
using CampFitFurDogs.Api.Horizontals.Startup.Modules;

namespace CampFitFurDogs.Api.Horizontals.Startup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupModules(
        this IServiceCollection services)
    {
        services.AddSingleton<IStartupModule, EndpointsStartupModule>();
        services.AddSingleton<IStartupModule, ApplicationStartupModule>();
        services.AddSingleton<IStartupModule, AuthenticationStartupModule>();
        services.AddSingleton<IStartupModule, AuthorizationStartupModule>();
        services.AddSingleton<IStartupModule, CorsStartupModule>();
        services.AddSingleton<IStartupModule, ExceptionsStartupModule>();
        services.AddSingleton<IStartupModule, InfrastructureStartupModule>();
        services.AddSingleton<IStartupModule, LoggingStartupModule>();
        services.AddSingleton<IStartupModule, ObservationsStartupModule>();
        services.AddSingleton<IStartupModule, SecurityHeadersStartupModule>();
        services.AddSingleton<IStartupModule, SwaggerStartupModule>();

        return services;
    }
}
