using Frank.Abstractions.Startup;
using Frank.Infrastructure.Observability;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(30)]
public sealed class ObservabilityStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddObservability();
    }

    public void Use(WebApplication app)
    {
        app.UseObservability();
    }
}
