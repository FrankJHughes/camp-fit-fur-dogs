using CampFitFurDogs.Application;

namespace CampFitFurDogs.Api.Horizontals.Startup;

[StartupModule(70)]
public static class ApplicationStartupModule
{
    public static void Add(IServiceCollection services, IConfiguration config)
    {
        // Registers:
        // - MediatR handlers
        // - Validators
        // - Pipeline behaviors
        // - Application services
        // - Any other Application-layer dependencies
        services.AddApplication();
    }

    public static void Use(WebApplication app)
    {
        // Application layer has no middleware.
    }
}
