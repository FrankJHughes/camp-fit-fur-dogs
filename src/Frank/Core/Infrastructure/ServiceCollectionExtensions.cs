using Frank.Core.Infrastructure.EnvironmentVariables;
using Frank.Core.Infrastructure.Clock;
using Microsoft.Extensions.DependencyInjection;
using Frank.Core.Infrastructure.Observations;
using Frank.Core.Infrastructure.Exceptions;

namespace Frank.Core.Infrastructure;

/// <summary>
/// Provides a single entry point for registering all Infrastructure‑level
/// services into the dependency injection container.
/// <para>
/// This method wires up the concrete implementations for the platform’s
/// cross‑cutting concerns, including:
/// <list type="bullet">
/// <item><description>Clock (system time abstraction)</description></item>
/// <item><description>Environment variables and hosting metadata</description></item>
/// <item><description>Exception handler discovery and registry</description></item>
/// <item><description>Observability (contexts, sinks, metrics, correlation)</description></item>
/// </list>
/// </para>
/// <para>
/// Infrastructure contains no business logic; it provides runtime behavior
/// for abstractions defined in <c>Frank.Core.Application</c>.
/// Calling this method ensures the entire Infrastructure layer is activated
/// with a single, cohesive registration step.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Infrastructure subsystems into the provided
    /// <see cref="IServiceCollection"/>.
    /// <para>
    /// This includes:
    /// <list type="bullet">
    /// <item><description>Clock services</description></item>
    /// <item><description>Environment variable access</description></item>
    /// <item><description>Exception handler registry + discovery</description></item>
    /// <item><description>Observability (contexts, sinks, metrics)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// This method should be called once during application startup to ensure
    /// all Infrastructure components are available to the rest of the system.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The DI service collection to modify.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with all Infrastructure
    /// services registered.
    /// </returns>
    public static IServiceCollection AddFrankCoreInfrastructure(this IServiceCollection services)
    {
        return services
            .AddFrankCoreInfrastructureClock()
            .AddFrankCoreInfrastructureEnvironmentVariables()
            .AddFrankCoreInfrastructureExceptionHandlers()
            .AddFrankCoreInfrastructureObservations();
    }
}
