using System.Reflection;
using Frank.Abstractions.Startup;
using Frank.Infrastructure.Exceptions;
using Frank.Api.Exceptions.Middleware;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(20)]
public sealed class ExceptionsStartupModule : IStartupModule
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

        services.AddFrankException(assemblies);
    }

    public void Use(WebApplication app)
    {
        app.UseFrankExceptions();
    }
}
