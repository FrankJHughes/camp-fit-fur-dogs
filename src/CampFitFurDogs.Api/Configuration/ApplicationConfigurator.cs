using CampFitFurDogs.Application;

namespace CampFitFurDogs.Api.Configuration;

[Configurator(50)]
public static class ApplicationConfigurator
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        // Registers:
        // - MediatR handlers
        // - Validators
        // - Pipeline behaviors
        // - Application services
        // - Any other Application-layer dependencies
        services.AddApplication();
    }

    public static void Configure(WebApplication app)
    {
        // Application layer has no middleware.
    }
}
