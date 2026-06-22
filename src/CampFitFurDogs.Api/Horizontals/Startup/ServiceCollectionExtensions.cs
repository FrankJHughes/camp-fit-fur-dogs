using Frank.Abstractions.Startup;
using CampFitFurDogs.Api.Horizontals.Startup.Modules;

namespace CampFitFurDogs.Api.Horizontals.Startup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupModules(
        this IServiceCollection services)
    {
        services.AddSingleton<IStartupModule, ApiStartupModule>();
        services.AddSingleton<IStartupModule, ApplicationStartupModule>();
        services.AddSingleton<IStartupModule, AuthenticationStartupModule>();
        services.AddSingleton<IStartupModule, AuthorizationStartupModule>();
        services.AddSingleton<IStartupModule, CorsStartupModule>();
        services.AddSingleton<IStartupModule, ExceptionHandlingStartupModule>();
        services.AddSingleton<IStartupModule, InfrastructureStartupModule>();
        services.AddSingleton<IStartupModule, LoggingStartupModule>();
        services.AddSingleton<IStartupModule, ObservabilityStartupModule>();
        services.AddSingleton<IStartupModule, SecurityHeadersStartupModule>();
        services.AddSingleton<IStartupModule, SwaggerStartupModule>();

        return services;
    }
}
