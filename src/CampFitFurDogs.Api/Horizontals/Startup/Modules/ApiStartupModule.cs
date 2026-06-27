using System.Reflection;
using FluentValidation;
using Frank.Abstractions.Startup;
using Frank.Api.Endpoints;
using Frank.Infrastructure.Problem;


namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(110)]
public class ApiStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        var assemblies = new Assembly[]
        {
            typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly
        };

        services.AddValidatorsFromAssemblies(assemblies);

    }

    public void Use(WebApplication app)
    {
        // Endpoint discovery (Frank.Api)
        var apiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;
        EndpointDiscovery.AddEndpoints(apiAssembly);

        app.MapEndpoints();

        app.MapGet("/api/health", () => Results.Ok(new { Status = "Healthy" }))
           .WithName("HealthCheck");

    }

}
