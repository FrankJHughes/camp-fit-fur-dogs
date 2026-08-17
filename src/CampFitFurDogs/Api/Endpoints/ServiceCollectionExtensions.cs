using CampFitFurDogs.Api.Endpoints.Dogs;
using CampFitFurDogs.Api.Endpoints.Health;

namespace CampFitFurDogs.Api.Endpoints;

/// <summary>
/// Provides extension methods for registering all API endpoint groups within the
/// Camp Fit Fur Dogs application.
/// <para>
/// This class aggregates the endpoint registration methods for each feature area
/// (e.g., Dogs, Health) and exposes a single entry point for configuring the full
/// API surface.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Camp Fit Fur Dogs API endpoints by invoking the feature‑specific
    /// endpoint registration extensions.
    /// <para>
    /// This method ensures that all endpoints under:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>CampFitFurDogs.Api.Endpoints.Dogs</c></description></item>
    /// <item><description><c>CampFitFurDogs.Api.Endpoints.Health</c></description></item>
    /// </list>
    /// <para>
    /// are discovered and mapped through the Frank.Core endpoint discovery system.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with all API endpoints registered.
    /// </returns>
    public static IServiceCollection AddCampFitFurDogsApiEndpoints(
        this IServiceCollection services)
    {
        return services
            .AddCampFitFurDogsApiEndpointsDogs()
            .AddCampFitFurDogsApiEndpointsHealth();
    }
}
