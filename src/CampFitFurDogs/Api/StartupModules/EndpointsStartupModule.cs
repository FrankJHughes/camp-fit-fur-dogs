using FluentValidation;
using Frank.Core.Api.Endpoints;
using Frank.Core.Application.Abstractions.Startup;
using Frank.Identity.Api.Endpoints;


namespace CampFitFurDogs.Api.StartupModules;

[StartupModule(120)]
public class EndpointsStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddFrankIdentityApiEndpoints();

        services.AddValidatorsFromAssemblies([
            typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly]);

        services.AddFrankCoreApiEndpoints([
            typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly]);

    }

    public void Use(WebApplication app)
    {
        app.MapFrankCoreApiEndpoints();
    }

}
