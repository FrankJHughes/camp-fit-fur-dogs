using System.Reflection;
using Frank.Abstractions.Startup;
using Frank.Api.Exceptions.Middleware;
using Frank.Infrastructure.Exceptions;

namespace CampFitFurDogs.Api.StartupModules;

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
