using Frank.Core.Application.Abstractions.Startup;
using Frank.Identity.Api.Middleware.Sessions;


namespace CampFitFurDogs.Api.StartupModules;

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
