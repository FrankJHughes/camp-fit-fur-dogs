using Frank.Abstractions.Startup;
using CampFitFurDogs.Api.Horizontals.Startup.Modules;
using Frank.Api.Startup;

namespace CampFitFurDogs.Api.Horizontals.Startup;

public static class Startup
{
    public static IStartupModule[] ConstructStartupModules()
    {
        return
        [
            new ApiStartupModule(),
            new ApplicationStartupModule(),
            new AuthenticationStartupModule(),
            new AuthorizationStartupModule(),
            new CorsStartupModule(),
            new ExceptionHandlingStartupModule(),
            new IdentityStartupModule(),
            new InfrastructureStartupModule(),
            new LoggingStartupModule(),
            new ObservabilityStartupModule(),
            new SecurityHeadersStartupModule(),
            new SwaggerStartupModule()
        ];
    }

    public static void AddAllServices(WebApplicationBuilder builder)
    {
        var startupModules = ConstructStartupModules();
        var startupEngine = new StartupEngine(startupModules);
        startupEngine.AddAll(builder);

        builder.Services.AddStartupModules();
        builder.Services.AddStartupEngine();
    }

    public static void UseAllServices(WebApplication app)
    {
        var startupEngine = app.Services.GetRequiredService<StartupEngine>();
        startupEngine.UseAll(app);
    }

}
