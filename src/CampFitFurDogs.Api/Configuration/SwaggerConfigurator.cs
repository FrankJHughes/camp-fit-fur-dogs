namespace CampFitFurDogs.Api.Configuration;

[Configurator(40)]
public static class SwaggerConfigurator
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddOpenApi();
    }

    public static void Configure(WebApplication app)
    {
        // Only map OpenAPI in development
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
    }
}
