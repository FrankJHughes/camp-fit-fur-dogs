using CampFitFurDogs.Api.Configuration;

[Configurator(5)]
public static class KestrelHostingConfigurator
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config) { }

    public static void Configure(WebApplication app)
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

        app.Urls.Clear();
        app.Urls.Add($"http://0.0.0.0:{port}");
    }
}
