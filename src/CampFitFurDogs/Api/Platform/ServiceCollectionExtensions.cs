using CampFitFurDogs.Application;
using CampFitFurDogs.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api.Platform;

/// <summary>
/// Provides dependency‑injection extensions for configuring the Camp Fit Fur Dogs
/// API platform layer.
/// <para>
/// This module composes the three major subsystems required for API operation:
/// <list type="bullet">
/// <item><description><strong>Application</strong> — business logic, commands, handlers, and workflows.</description></item>
/// <item><description><strong>Infrastructure</strong> — persistence, observability, identity, and external integrations.</description></item>
/// <item><description><strong>API</strong> — endpoint abstractions, validators, exception handlers, and request pipelines.</description></item>
/// </list>
/// Calling this method ensures that all platform‑level services are registered
/// consistently during application startup.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all Camp Fit Fur Dogs platform‑level services to the provided
    /// <see cref="IServiceCollection"/>, including the Application layer,
    /// Infrastructure layer, and API layer.
    /// <para>
    /// This method should be invoked once during application startup to ensure
    /// that the API is fully composed and ready to process requests.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to configure.
    /// </param>
    /// <param name="configuration">
    /// The application configuration used by the Infrastructure layer.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> containing all platform
    /// services required for the Camp Fit Fur Dogs API.
    /// </returns>
    public static IServiceCollection AddCampFitFurDogsApiPlatform(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            // Application layer (commands, handlers, workflows)
            .AddCampFitFurDogsApplication()

            // Infrastructure layer (persistence, observability, identity, etc.)
            .AddCampFitFurDogsInfrastructure(configuration)

            // API layer (validators, exception handlers, endpoint abstractions)
            .AddCampFitFurDogsApi();
    }
}
