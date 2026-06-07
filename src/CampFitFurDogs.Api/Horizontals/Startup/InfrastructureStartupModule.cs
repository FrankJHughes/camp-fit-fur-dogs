using CampFitFurDogs.Infrastructure;

namespace CampFitFurDogs.Api.Horizontals.Startup;

[StartupModule(90)]
public static class InfrastructureStartupModule
{
    public static void Add(IServiceCollection services, IConfiguration config)
    {
        // This registers:
        // - EF Core DbContext
        // - Repositories
        // - AuthCallbackService dependencies
        // - Email providers
        // - External API clients
        // - Any other infrastructure services
        services.AddInfrastructure(config);
    }

    public static void Use(WebApplication app)
    {
        // Infrastructure typically has no middleware.
        // If you later add outbox workers or background services,
        // they will be registered in Add, not here.
    }
}
