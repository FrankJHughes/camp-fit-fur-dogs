using Frank.Core.Api.Endpoints;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api.Endpoints.Dogs;

/// <summary>
/// Provides extension methods for registering all dog‑related API endpoints
/// within the Camp Fit Fur Dogs application.
/// <para>
/// This class integrates the Dogs endpoint assembly into the Frank.Core API
/// endpoint discovery pipeline, ensuring that all <see cref="IEndpoint"/>
/// implementations under the <c>CampFitFurDogs.Api.Endpoints.Dogs</c> namespace
/// are automatically discovered and mapped.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all dog‑related API endpoints by scanning the assembly containing
    /// <see cref="CampFitFurDogs.Api.AssemblyMarker"/> and applying a namespace
    /// filter to include only endpoint implementations under the Dogs namespace.
    /// <para>
    /// This method configures <see cref="DiscoveryOptions"/> to restrict discovery
    /// to types whose namespaces begin with the namespace of this class, ensuring
    /// that only dog‑specific endpoints are included.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with dog‑related endpoints registered.
    /// </returns>
    public static IServiceCollection AddCampFitFurDogsApiEndpointsDogs(
        this IServiceCollection services)
    {
        static DiscoveryOptions updateOptions(DiscoveryOptions options) =>
            options.IncludeImplementations(t =>
                !string.IsNullOrWhiteSpace(t.Namespace) &&
                t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services
            .AddFrankCoreApiEndpoints(
                assembliesToSearch: [typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly],
                configure: options => updateOptions(options)
            );
    }
}
