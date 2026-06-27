using Frank.Abstractions.Startup;
using Frank.Api.SecurityHeaders;

namespace CampFitFurDogs.Api.Horizontal.Startup.Modules;

[StartupModule(40)]
public sealed class SecurityHeadersStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddSecurityHeaders();
    }

    public void Use(WebApplication app) { }
}
