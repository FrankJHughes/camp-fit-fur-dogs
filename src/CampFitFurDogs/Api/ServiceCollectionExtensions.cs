using CampFitFurDogs.Api.ExceptionHandlers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api;

/// <summary>
/// Provides dependency‑injection extensions for configuring the Camp Fit Fur Dogs
/// API layer.
/// <para>
/// This module registers API‑level components such as exception handlers and
/// request validators. It should be invoked once during application startup to
/// ensure consistent API behavior across all endpoints.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all API‑level services required by Camp Fit Fur Dogs, including
    /// exception handlers and FluentValidation validators.
    /// <para>
    /// Validators are automatically discovered from the assembly containing
    /// <see cref="AssemblyMarker"/>, ensuring that all endpoint request DTOs
    /// have their corresponding validators registered.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to configure.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with all API services
    /// registered.
    /// </returns>
    public static IServiceCollection AddCampFitFurDogsApi(this IServiceCollection services)
    {
        return services
            // API-level exception handlers
            .AddCampFitFurDogsApiExceptionHandlers()

            // Automatically register all FluentValidation validators
            .AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
    }
}
