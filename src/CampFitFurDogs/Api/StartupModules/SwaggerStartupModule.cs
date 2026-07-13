using Frank.Core.Application.Abstractions.Startup;

namespace CampFitFurDogs.Api.StartupModules;

[StartupModule(60)]
public class SwaggerStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddOpenApi();
    }

    public void Use(WebApplication app)
    {
        // Only map OpenAPI in development
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
    }
}
