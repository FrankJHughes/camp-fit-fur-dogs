using Frank.Core.Api.Middleware.Observations;
using Frank.Core.Application.Abstractions.Startup;

namespace CampFitFurDogs.Api.StartupModules;

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
