using CampFitFurDogs.Api.HostingModules;
using Frank.Abstractions.Hosting;
using Frank.Api.Hosting;

namespace CampFitFurDogs.Api.Helpers;

public static class Hosting
{
    public static IHostingModule[] ConstructHostingModules()
    {
        return
        [
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
