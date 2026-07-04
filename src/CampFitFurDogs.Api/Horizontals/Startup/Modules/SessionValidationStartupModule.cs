using CampFitFurDogs.Api.Horizontals.Session.Middleware;
using Frank.Abstractions.Startup;


namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(75)]
public class SessionValidationStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
    }

    public void Use(WebApplication app)
    {
        app.UseSessionValidation();
    }

}
