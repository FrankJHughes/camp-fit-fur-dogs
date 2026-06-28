using CampFitFurDogs.Infrastructure;
using Frank.Abstractions.Startup;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(100)]
public class InfrastructureStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;

        // This registers:
        // - EF Core DbContext
        // - Repositories
        // - AuthCallbackService dependencies
        // - Email providers
        // - External API clients
        // - Any other infrastructure services
        services.AddInfrastructure(config);
    }

    public void Use(WebApplication app)
    {
        // Infrastructure typically has no middleware.
        // If you later add outbox workers or background services,
        // they will be registered in Add, not here.
    }
}
