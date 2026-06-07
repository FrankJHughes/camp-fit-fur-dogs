using Microsoft.AspNetCore.Authorization;

namespace CampFitFurDogs.Api.Configuration;

[Configurator(30)]
public static class AuthorizationConfigurator
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddAuthorization(options =>
        {
            // Future policies go here (US‑110, US‑111, US‑133)
        });
    }

    public static void Configure(WebApplication app)
    {
        app.UseAuthorization();
    }
}
