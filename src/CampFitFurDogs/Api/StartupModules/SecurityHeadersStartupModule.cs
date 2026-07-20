using Frank.Core.Api.Middleware.SecurityHeaders;
using Frank.Core.Application.Abstractions.Startup;

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
