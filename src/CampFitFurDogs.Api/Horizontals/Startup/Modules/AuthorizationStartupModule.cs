using Frank.Abstractions.Startup;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(50)]
public class AuthorizationStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddAuthorization(options =>
        {
            // Future policies go here (US‑110, US‑111, US‑133)
        });
    }

    public void Use(WebApplication app)
    {
        app.UseAuthorization();
    }
}
