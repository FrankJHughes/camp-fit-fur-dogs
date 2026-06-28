using Frank.Abstractions.Hosting;

namespace CampFitFurDogs.Api.Horizontals.Hosting.Modules;

[HostingModule(0)]
public sealed class LocalDevelopmentHostingModule : IHostingModule
{
    public int Order => 0;

    public string ProviderName => "Local Development Hosting Provider";

    public bool IsActive(WebApplicationBuilder builder)
        => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

    public async Task<IDictionary<string, string?>> GetConfigurationOverridesAsync(WebApplicationBuilder builder)
    {
        {
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            return new Dictionary<string, string?>
            {
                ["Frontend:BaseUrl"] = $"https://localhost:5173"
            };
        }
    }

}
