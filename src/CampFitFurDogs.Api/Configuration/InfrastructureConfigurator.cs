using CampFitFurDogs.Infrastructure;

namespace CampFitFurDogs.Api.Configuration;

[Configurator(70)]
public static class InfrastructureConfigurator
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
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

    public static void Configure(WebApplication app)
    {
        // Infrastructure typically has no middleware.
        // If you later add outbox workers or background services,
        // they will be registered in ConfigureServices, not here.
    }
}
