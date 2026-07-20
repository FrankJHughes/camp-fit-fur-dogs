using CampFitFurDogs.Api.StartupModules;
using Frank.Core.Api.Startup;
using Frank.Core.Application.Abstractions.Startup;

namespace CampFitFurDogs.Api.Helpers;

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
            new ObservationsStartupModule(),
            new SecurityHeadersStartupModule(),
            new SessionValidationStartupModule(),
            new SwaggerStartupModule(),
            new ValidatorsStartupModule()
        ];
    }

    public static void AddAllServices(WebApplicationBuilder builder)
    {
        var startupModules = ConstructStartupModules();
        var startupEngine = new StartupEngine(startupModules);

        // Run Add on ALL modules
        startupEngine.AddAll(builder);

        // Register THIS engine in DI so UseAllServices sees the same module set
        builder.Services.AddSingleton(startupEngine);
    }

    public static void UseAllServices(WebApplication app)
    {
        var startupEngine = app.Services.GetRequiredService<StartupEngine>();
        startupEngine.UseAll(app);
    }
}
