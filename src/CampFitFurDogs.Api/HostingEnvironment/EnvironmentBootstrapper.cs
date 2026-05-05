namespace CampFitFurDogs.Api.HostingEnvironment;

public static class EnvironmentBootstrapper
{
    /// <summary>
    /// Applies all environment-specific configuration overrides.
    /// Must be called immediately after creating the WebApplicationBuilder.
    /// </summary>
    public static async Task ApplyOverridesAsync(WebApplicationBuilder builder)
    {
        // Add future overrides here in a clean, composable way.
        await PreviewDatabaseOverride.ApplyAsync(builder);
    }
}
