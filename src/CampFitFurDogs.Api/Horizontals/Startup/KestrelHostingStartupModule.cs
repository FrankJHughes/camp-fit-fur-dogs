using CampFitFurDogs.Api.Horizontals.Startup;

[StartupModule(30)]
public static class KestrelHostingStartupModule
{
    public static void Add(IServiceCollection services, IConfiguration config) { }

    public static void Use(WebApplication app)
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

        app.Urls.Clear();
        app.Urls.Add($"http://0.0.0.0:{port}");
    }
}
