using Frank.Api;
using Frank.DependencyInjection;

using Frank.Api.SecurityHeaders;

namespace CampFitFurDogs.Api.Horizontals.Startup;

[StartupModule(100)]
public static class ApiStartupModule
{
    public static void Add(IServiceCollection services, IConfiguration config)
    {
        // Register API-layer validators, request DTO conventions, etc.
        services.AddFrank([
            typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly
        ]);

        services.AddSecurityHeaders();
    }

    public static void Use(WebApplication app)
    {
        // Endpoint discovery (Frank.Api)
        var apiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;
        EndpointDiscovery.AddEndpoints(apiAssembly);

        app.MapEndpoints();

        app.MapGet("/api/health", () => Results.Ok(new { Status = "Healthy" }))
           .WithName("HealthCheck");

    }
}
