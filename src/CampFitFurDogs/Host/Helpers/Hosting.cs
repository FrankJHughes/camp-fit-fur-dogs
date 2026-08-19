using Frank.Core.Application.Abstractions.Hosting;
using Frank.Core.Api.HostingModules;
using CampFitFurDogs.Api.HostingModules;

namespace CampFitFurDogs.Host.Helpers;

/// <summary>
/// Provides helper methods for constructing and applying hosting‑module
/// configuration within the Camp Fit Fur Dogs API.
/// <para>
/// Hosting modules allow the application to adapt its behavior based on the
/// environment in which it is running (local development, Render PR preview,
/// production, etc.).
/// This class centralizes the creation of hosting modules and applies them
/// through the <see cref="HostingEngine"/>.
/// </para>
/// </summary>
public static class Hosting
{
    /// <summary>
    /// Constructs the set of hosting modules used by the Camp Fit Fur Dogs API.
    /// <para>
    /// Each hosting module encapsulates environment‑specific configuration logic.
    /// Currently, this includes the <see cref="RenderPrPreviewHostingModule"/>,
    /// which adjusts settings when running inside Render PR preview environments.
    /// </para>
    /// </summary>
    /// <returns>
    /// An array of <see cref="IHostingModule"/> instances representing the hosting
    /// configuration modules to be applied.
    /// </returns>
    public static IHostingModule[] ConstructHostingModules()
    {
        return
        [
            new RenderPrPreviewHostingModule()
        ];
    }

    /// <summary>
    /// Applies hosting‑environment configuration to the provided
    /// <see cref="WebApplicationBuilder"/> using the configured hosting modules.
    /// <para>
    /// This method constructs the hosting modules, initializes a
    /// <see cref="HostingEngine"/>, and executes environment‑specific configuration
    /// logic asynchronously.
    /// </para>
    /// </summary>
    /// <param name="builder">
    /// The <see cref="WebApplicationBuilder"/> to adapt based on the hosting environment.
    /// </param>
    public static async Task AdaptToHostingEnvironment(WebApplicationBuilder builder)
    {
        var hostingModules = Hosting.ConstructHostingModules();
        var hostingEngine = new HostingEngine(hostingModules);
        await hostingEngine.ApplyHostingEnvironmentConfigurationAsync(builder);
    }
}
