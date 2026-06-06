using CampFitFurDogs.Api.Configuration;
using CampFitFurDogs.Api.Hosting;
using Frank.Abstractions.Environment;
using Frank.Api.Hosting;
using Frank.Infrastructure.Environment;

[Configurator(0)]
public static class HostingProviderConfigurator
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
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

    public static void Configure(WebApplication app) { }
}
