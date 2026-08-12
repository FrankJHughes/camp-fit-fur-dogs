using CampFitFurDogs.Api.ExceptionHandlers;
using CampFitFurDogs.Application;
using CampFitFurDogs.Infrastructure;

namespace CampFitFurDogs.Api.Platform;

/// <summary>
/// Provides extension methods for registering the full Camp Fit Fur Dogs API
/// platform, including application services, infrastructure components, and
/// API‑level exception handlers.
/// <para>
/// This extension method centralizes all platform‑level registrations so that
/// <c>Program.cs</c> remains minimal and declarative.
/// It composes the three major layers:
/// <list type="bullet">
/// <item><description>Application layer</description></item>
/// <item><description>Infrastructure layer</description></item>
/// <item><description>API exception‑handling layer</description></item>
/// </list>
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all platform‑level services required by the Camp Fit Fur Dogs API.
    /// <para>
    /// This includes:
    /// <list type="bullet">
    /// <item><description>Application services (<see cref="AddCampFitFurDogsApplication"/>)</description></item>
    /// <item><description>Infrastructure services (<see cref="AddCampFitFurDogsInfrastructure"/>)</description></item>
    /// <item><description>API exception handlers (<see cref="AddCampFitFurDogsApiExceptionHandlers"/>)</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <param name="configuration">The application configuration root.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with all platform components registered.
    /// </returns>
    public static IServiceCollection AddCampFitFurDogsApiPlatform(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddCampFitFurDogsApplication()
            .AddCampFitFurDogsInfrastructure(configuration)
            .AddCampFitFurDogsApiExceptionHandlers();
    }
}
