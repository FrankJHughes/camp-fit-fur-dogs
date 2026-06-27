using System.Reflection;
using Frank.Abstractions.Startup;
using Frank.Infrastructure.Problem;
using Frank.Api.ExceptionHandling;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(20)]
public sealed class ExceptionHandlingStartupModule : IStartupModule
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
