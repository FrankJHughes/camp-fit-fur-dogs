using Frank;
using Frank.Abstractions.Startup;
using Frank.Api.Endpoints;
using Microsoft.AspNetCore.HttpOverrides;


namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(110)]
public class ApiStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        // Register API-layer validators, request DTO conventions, etc.
        services.AddFrank([
            typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly
        ]);
    }

    public void Use(WebApplication app)
    {
        app.UseForwardedHeaders();

        // Endpoint discovery (Frank.Api)
        var apiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;
        EndpointDiscovery.AddEndpoints(apiAssembly);

        app.MapEndpoints();

        app.MapGet("/api/health", () => Results.Ok(new { Status = "Healthy" }))
           .WithName("HealthCheck");

    }

}
