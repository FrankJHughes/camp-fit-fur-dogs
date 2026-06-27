using Frank.Abstractions.Startup;
using Frank.Infrastructure.Observability;

namespace CampFitFurDogs.Api.Horizontal.Startup.Modules;

[StartupModule(30)]
public sealed class ObservabilityStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddFrankObservability();
    }

    public void Use(WebApplication app)
    {
        app.UseFrankObservability();
    }
}
