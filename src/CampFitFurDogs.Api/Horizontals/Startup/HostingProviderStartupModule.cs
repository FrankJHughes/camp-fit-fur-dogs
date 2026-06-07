using CampFitFurDogs.Api.Horizontals.Startup;
using CampFitFurDogs.Api.Horizontals.Hosting;
using Frank.Abstractions.Environment;
using Frank.Api.Hosting;
using Frank.Infrastructure.Environment;

[StartupModule(0)]
public static class HostingProviderStartupModule
{
    public static void Add(IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IEnvironment, SystemEnvironment>();
        services.AddSingleton<IRenderPrParser, RenderPrParser>();

        services.AddSingleton<IGitHubArtifactClient>(_ =>
            RenderHostingProvider.HttpClientFactoryOverride is null
                ? new GitHubArtifactClient()
                : new GitHubArtifactClient(RenderHostingProvider.HttpClientFactoryOverride));

        services.AddSingleton<IRenderConfigurationWriter, RenderConfigurationWriter>();
        services.AddSingleton<IHostingProvider, RenderHostingProvider>();

    }

    public static void Use(WebApplication app) { }
}
