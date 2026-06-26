using CampFitFurDogs.Application;
using Frank.Abstractions.Startup;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(90)]
public class ApplicationStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        // Registers:
        // - MediatR handlers
        // - Validators
        // - Pipeline behaviors
        // - Application services
        // - Any other Application-layer dependencies
        services.AddApplication();
    }

    public void Use(WebApplication app)
    {
        // Application layer has no middleware.
    }
}
