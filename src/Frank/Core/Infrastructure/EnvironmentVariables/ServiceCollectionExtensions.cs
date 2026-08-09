using Frank.Core.Application.Abstractions.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Infrastructure.EnvironmentVariables;

/// <summary>
/// Provides DI registration extensions for the infrastructure‑level
/// environment variable provider.
/// <para>
/// This module registers <see cref="SystemEnvironmentVariables"/> as the
/// default <see cref="IEnvironmentVariables"/> implementation, allowing
/// application and slice code to retrieve environment variables through an
/// abstraction rather than directly accessing <see cref="System.Environment"/>.
/// </para>
/// <para>
/// Test environments may override this registration with in‑memory or
/// deterministic implementations to ensure predictable behavior.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="SystemEnvironmentVariables"/> as the scoped
    /// implementation of <see cref="IEnvironmentVariables"/>.
    /// </summary>
    /// <param name="services">
    /// The service collection to modify.
    /// </param>
    /// <returns>
    /// The updated service collection.
    /// </returns>
    public static IServiceCollection AddFrankCoreInfrastructureEnvironmentVariables(this IServiceCollection services)
    {
        services.AddScoped<IEnvironmentVariables, SystemEnvironmentVariables>();
        return services;
    }
}
