using Frank.Core.Application.Registration;
using Frank.Core.Infrastructure.Exceptions;

namespace CampFitFurDogs.Api.ExceptionHandlers;

/// <summary>
/// Provides extension methods for registering all exception handlers used by the
/// Camp Fit Fur Dogs API.
/// <para>
/// This class integrates the exception handler assembly into the Frank.Core
/// infrastructure exception‑handling discovery pipeline, ensuring that all
/// <see cref="IExceptionHandler"/> implementations under the
/// <c>CampFitFurDogs.Api.ExceptionHandlers</c> namespace are automatically
/// discovered and registered.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all API exception handlers by scanning the assembly containing
    /// <see cref="CampFitFurDogs.Api.AssemblyMarker"/> and applying a namespace
    /// filter to include only handler implementations under the
    /// <c>CampFitFurDogs.Api.ExceptionHandlers</c> namespace.
    /// <para>
    /// This method configures <see cref="DiscoveryOptions"/> to restrict discovery
    /// to types implementing exception‑handler interfaces, ensuring that only
    /// relevant handlers are included.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with all exception handlers registered.
    /// </returns>
    public static IServiceCollection AddCampFitFurDogsApiExceptionHandlers(
        this IServiceCollection services)
    {
        static DiscoveryOptions updateOptions(DiscoveryOptions options) =>
            options.IncludeInterfaces(t =>
                !string.IsNullOrWhiteSpace(t.Namespace) &&
                t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services
            .AddFrankCoreInfrastructureExceptionHandlers(
                assembliesToSearch: [typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly],
                configure: options => updateOptions(options)
            );
    }
}
