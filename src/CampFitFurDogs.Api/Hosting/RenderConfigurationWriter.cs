using Microsoft.AspNetCore.Builder;

namespace CampFitFurDogs.Api.Hosting;

public sealed class RenderConfigurationWriter : IRenderConfigurationWriter
{
    private const string ConfigKey_DbConn = "ConnectionStrings:DefaultConnection";
    private const string ConfigKey_FrontendBaseUrl = "Frontend__BaseUrl";

    public void Apply(WebApplicationBuilder builder, string dbConnectionString, string frontendBaseUrl)
    {
        builder.Configuration[ConfigKey_DbConn] = dbConnectionString;
        builder.Configuration[ConfigKey_FrontendBaseUrl] = frontendBaseUrl;

        Log("DB connection string overridden from GitHub artifact.");
        Log($"Frontend base URL overridden from GitHub artifact: {frontendBaseUrl}.");
    }

    private static void Log(string message)
        => Console.WriteLine($"[Hosting:Render:Config] {message}");
}
