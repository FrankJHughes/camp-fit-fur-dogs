using System.Reflection;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Endpoints;

/// <summary>
/// Provides extension methods for discovering and registering API endpoint
/// implementations using the Frank.Core registration orchestrator.
/// <para>
/// This enables vertical‑slice endpoint discovery: any interface decorated with
/// <see cref="RegistrationAttribute"/> and matching <see cref="IEndpoint"/> will
/// have its implementations automatically registered into the DI container.
/// </para>
/// <para>
/// The orchestrator performs reflection‑based discovery across the specified
/// assemblies, applying the configured <see cref="DiscoveryOptions"/> filters.
/// </para>
/// </summary>
public static class EndpointServiceCollectionExtensions
{
    /// <summary>
    /// Discovers and registers all <see cref="IEndpoint"/> implementations found
    /// in the provided assemblies, using the Frank.Core registration orchestrator.
    /// <para>
    /// Discovery rules:
    /// <list type="bullet">
    /// <item><description>
    /// Interfaces must be <see cref="IEndpoint"/> and decorated with
    /// <see cref="RegistrationAttribute"/>.
    /// </description></item>
    /// <item><description>
    /// Implementations must implement <see cref="IEndpoint"/>.
    /// </description></item>
    /// </list>
    /// </para>
    /// <para>
    /// If no assemblies are provided, the method performs no registration and
    /// returns the original <see cref="IServiceCollection"/>.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The DI service collection to modify.
    /// </param>
    /// <param name="assembliesToSearch">
    /// Optional assemblies to scan for endpoint interfaces and implementations.
    /// If null or empty, discovery is skipped.
    /// </param>
    /// <param name="configure">
    /// Optional delegate allowing customization of <see cref="DiscoveryOptions"/>
    /// before orchestrator execution.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with discovered endpoints
    /// registered.
    /// </returns>
    public static IServiceCollection AddFrankCoreApiEndpoints(
        this IServiceCollection services,
        IEnumerable<Assembly>? assembliesToSearch = null,
        Action<DiscoveryOptions>? configure = null)
    {
        if (assembliesToSearch is null || !assembliesToSearch.Any())
        {
            return services;
        }

        var options = new DiscoveryOptions();

        //
        // Interface must:
        //   - be IEndpoint
        //   - AND be decorated with [Registration]
        //
        options.IncludeInterfaces(iface =>
            iface.AsType() == typeof(IEndpoint) &&
            iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0);

        //
        // Implementations: any class implementing IEndpoint
        //
        options.IncludeImplementations(impl =>
            impl.ImplementedInterfaces.Any(i => i == typeof(IEndpoint)));

        configure?.Invoke(options);

        IEnumerable<Assembly> assemblies =
        [
            typeof(Frank.Core.Application.AssemblyMarker).Assembly,
            .. assembliesToSearch
        ];

        Orchestrator.Orchestrate(
            services,
            assemblies,
            options);

        return services;
    }
}
