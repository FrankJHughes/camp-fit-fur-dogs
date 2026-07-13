using CampFitFurDogs.Api.HostingModules;
using Frank.Core.Application.Abstractions.Hosting;
using Frank.Core.Api.Hosting;

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
