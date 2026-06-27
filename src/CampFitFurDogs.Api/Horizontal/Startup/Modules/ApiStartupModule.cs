using System.Reflection;
using FluentValidation;
using Frank.Abstractions.Startup;
using Frank.Api.Endpoints;


namespace CampFitFurDogs.Api.Horizontal.Startup.Modules;

[StartupModule(110)]
public class ApiStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddApi();
    }

    public void Use(WebApplication app)
    {
        // Endpoint discovery (Frank.Api)
        var apiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;
        EndpointDiscovery.AddEndpoints(apiAssembly);

        app.MapEndpoints();
    }

}
