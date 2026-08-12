using CampFitFurDogs.Application.Dogs;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application;

/// <summary>
/// Provides extension methods for registering all application‑layer components
/// for the CampFitFurDogs system.
/// <para>
/// This includes vertical‑slice CQRS registrations (commands, queries, handlers)
/// as well as FluentValidation validators discovered within the application
/// assembly.
/// </para>
/// <para>
/// The method centralizes application‑layer setup so that the API or hosting
/// layer can register all application services with a single call.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all CampFitFurDogs application‑layer services into the provided
    /// <see cref="IServiceCollection"/>.
    /// <para>
    /// This includes:
    /// <list type="bullet">
    /// <item><description>All Dogs vertical‑slice CQRS components via <c>AddApplicationDogs()</c>.</description></item>
    /// <item><description>All FluentValidation validators discovered in the application assembly.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// This method should be called once during application startup, typically
    /// from the API project's dependency‑injection configuration.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The dependency‑injection container to which application services will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    public static IServiceCollection AddCampFitFurDogsApplication(
        this IServiceCollection services)
    {
        services
            .AddApplicationDogs()
            .AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);

        return services;
    }
}
