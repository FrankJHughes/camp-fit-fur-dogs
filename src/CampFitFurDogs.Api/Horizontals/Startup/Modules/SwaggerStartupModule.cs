using Frank.Abstractions.Startup;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

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
