using Frank.Core.Api.Middleware.Observations;
using Frank.Core.Application.Abstractions.Startup;
using Frank.Identity.Api.Middleware.Observations;

namespace CampFitFurDogs.Api.StartupModules;

[StartupModule(30)]
public sealed class ObservationsStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddFrankCoreApiObservations();
    }

    public void Use(WebApplication app)
    {
        app
            .UseFrankCoreApiObservations()
            .UseFrankIdentityApiMiddlewareObservations();
    }
}
