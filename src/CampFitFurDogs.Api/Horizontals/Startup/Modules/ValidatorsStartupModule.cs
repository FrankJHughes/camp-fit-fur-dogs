using FluentValidation;
using Frank.Abstractions.Startup;


namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(110)]
public class ValidatorsStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddValidatorsFromAssemblies([
            typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly]);
    }

    public void Use(WebApplication app)
    {
        //
    }

}
