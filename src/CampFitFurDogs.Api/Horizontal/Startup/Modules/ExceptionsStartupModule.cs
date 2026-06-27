using System.Reflection;
using Frank.Abstractions.Startup;
using Frank.Infrastructure.Exceptions;
using Frank.Api.ExceptionHandling;

namespace CampFitFurDogs.Api.Horizontal.Startup.Modules;

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

        services.AddFrankProblem(assemblies);
    }

    public void Use(WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
