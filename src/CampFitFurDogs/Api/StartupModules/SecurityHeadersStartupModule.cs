using Frank.Core.Application.Abstractions.Startup;
using Frank.Core.Api.Middleware.SecurityHeaders;

namespace CampFitFurDogs.Api.StartupModules;

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
