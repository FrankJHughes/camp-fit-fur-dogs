using Microsoft.AspNetCore.Authorization;

namespace CampFitFurDogs.Api.Horizontals.Startup;

[StartupModule(50)]
public static class AuthorizationStartupModule
{
    public static void Add(IServiceCollection services, IConfiguration config)
    {
        services.AddAuthorization(options =>
        {
            // Future policies go here (US‑110, US‑111, US‑133)
        });
    }

    public static void Use(WebApplication app)
    {
        app.UseAuthorization();
    }
}
