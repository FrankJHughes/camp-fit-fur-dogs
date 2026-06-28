using Frank.Abstractions.Startup;
using Frank.Infrastructure.Observations;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(30)]
public sealed class ObservationsStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddFrankObservations();
    }

    public void Use(WebApplication app)
    {
        app.UseFrankObservations();
    }
}
