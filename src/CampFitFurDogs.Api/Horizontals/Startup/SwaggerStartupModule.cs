namespace CampFitFurDogs.Api.Horizontals.Startup;

[StartupModule(40)]
public static class SwaggerStartupModule
{
    public static void Add(IServiceCollection services, IConfiguration config)
    {
        services.AddOpenApi();
    }

    public static void Use(WebApplication app)
    {
        // Only map OpenAPI in development
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
    }
}
