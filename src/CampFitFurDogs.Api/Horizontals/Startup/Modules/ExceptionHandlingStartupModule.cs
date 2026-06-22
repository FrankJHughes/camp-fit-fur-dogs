using Frank.Abstractions.Startup;
using Frank.Api;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(20)]
public sealed class ExceptionHandlingStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddExceptionHandling(); // from Frank.Api
    }

    public void Use(WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
