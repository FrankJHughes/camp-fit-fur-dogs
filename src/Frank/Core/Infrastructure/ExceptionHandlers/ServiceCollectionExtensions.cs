using System.Reflection;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Infrastructure.Exceptions;

/// <summary>
/// Provides DI registration extensions for the exception‑handling subsystem.
/// <para>
/// This module registers the <see cref="ExceptionHandlerRegistry"/> and performs
/// attribute‑driven discovery of <see cref="IExceptionHandler"/> implementations
/// across assemblies.
/// </para>
/// <para>
/// Handlers must implement <see cref="IExceptionHandler"/> and be decorated with
/// <see cref="RegistrationAttribute"/> to be included in discovery.
/// This ensures explicit, slice‑controlled registration and prevents accidental
/// inclusion of internal or non‑public handlers.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Determines whether the specified interface is decorated with
    /// <see cref="RegistrationAttribute"/>.
    /// Used to filter interfaces during discovery.
    /// </summary>
    private static bool HasRegistrationAttribute(TypeInfo iface) =>
        iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0;

    /// <summary>
    /// Registers the <see cref="ExceptionHandlerRegistry"/> as a singleton.
    /// <para>
    /// The registry provides ordered resolution of exception handlers based on
    /// <see cref="ExceptionHandlerAttribute"/> metadata.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to modify.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddFrankCoreInfrastructureExceptionHandlerRegistry(this IServiceCollection services)
    {
        return services
            .AddSingleton<ExceptionHandlerRegistry>();
    }

    /// <summary>
    /// Discovers and registers <see cref="IExceptionHandler"/> implementations
    /// across the specified assemblies using the unified registration
    /// <see cref="Orchestrator"/> pipeline.
    /// <para>
    /// Only interfaces decorated with <see cref="RegistrationAttribute"/> and
    /// equal to <see cref="IExceptionHandler"/> are included.
    /// Implementations must explicitly implement <see cref="IExceptionHandler"/>.
    /// </para>
    /// <para>
    /// Additional discovery rules may be supplied via <paramref name="configure"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to modify.</param>
    /// <param name="assembliesToSearch">
    /// Assemblies to scan for exception handler interfaces and implementations.
    /// </param>
    /// <param name="configure">
    /// Optional callback allowing callers to customize <see cref="DiscoveryOptions"/>.
    /// </param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddFrankCoreInfrastructureExceptionHandlers(
        this IServiceCollection services,
        IEnumerable<Assembly>? assembliesToSearch = null,
        Action<DiscoveryOptions>? configure = null)
    {
        services.AddFrankCoreInfrastructureExceptionHandlerRegistry();

        if (assembliesToSearch is null || !assembliesToSearch.Any())
        {
            return services;
        }

        var options = new DiscoveryOptions();

        // Interface must:
        //   - be IExceptionHandler
        //   - AND be decorated with [Registration]
        options.IncludeInterfaces(iface =>
            HasRegistrationAttribute(iface) &&
            iface.AsType() == typeof(IExceptionHandler));

        // Implementations: any class implementing IExceptionHandler
        options.IncludeImplementations(impl =>
            impl.ImplementedInterfaces.Any(i =>
                i == typeof(IExceptionHandler)));

        // Allow user overrides
        configure?.Invoke(options);

        // Unified discovery + registration
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
