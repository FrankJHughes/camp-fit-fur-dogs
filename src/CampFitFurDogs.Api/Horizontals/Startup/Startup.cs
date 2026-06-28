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
            new EndpointsStartupModule(),
            new ApplicationStartupModule(),
            new AuthenticationStartupModule(),
            new AuthorizationStartupModule(),
            new CorsStartupModule(),
            new ExceptionsStartupModule(),
            new IdentityStartupModule(),
            new InfrastructureStartupModule(),
            new LoggingStartupModule(),
            new ObservabilityStartupModule(),
            new SecurityHeadersStartupModule(),
            new SwaggerStartupModule(),
            new ValidatorsStartupModule()
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
