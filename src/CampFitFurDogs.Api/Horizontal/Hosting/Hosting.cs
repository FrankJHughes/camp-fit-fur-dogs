using CampFitFurDogs.Api.Horizontal.Hosting.Modules;
using Frank.Abstractions.Hosting;
using Frank.Api.Hosting;

namespace CampFitFurDogs.Api.Horizontal.Hosting;

public static class Hosting
{
    public static IHostingModule[] ConstructHostingModules()
    {
        return
        [
            new LocalDevelopmentHostingModule(),
            new RenderPrPreviewHostingModule()
        ];
    }

    public static async Task AdaptToHostingEnvironment(WebApplicationBuilder builder)
    {
        var hostingModules = Hosting.ConstructHostingModules();
        var hostingEngine = new HostingEngine(hostingModules);
        await hostingEngine.ApplyHostingEnvironmentConfigurationAsync(builder);
    }

}
