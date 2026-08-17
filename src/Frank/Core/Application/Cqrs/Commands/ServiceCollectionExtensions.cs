using System.Reflection;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Frank.Core.Application.Cqrs.Commands;

/// <summary>
/// Provides extension methods for registering CQRS command handlers and the
/// command dispatcher into the dependency injection container.
///
/// <para>
/// These extensions support attribute‑driven discovery of command handler
/// interfaces and their implementations. Only interfaces decorated with
/// <see cref="RegistrationAttribute"/> and matching the generic shapes
/// <see cref="ICommandHandler{TCommand}"/> or
/// <see cref="ICommandHandler{TCommand, TResponse}"/> are included.
/// </para>
///
/// <para>
/// Implementations are discovered automatically based on the interfaces they
/// implement. This enables clean separation between handler contracts and
/// handler logic, while allowing flexible assembly scanning.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Determines whether the specified interface is decorated with
    /// <see cref="RegistrationAttribute"/>, indicating that it should be
    /// included in handler discovery.
    /// </summary>
    private static bool HasRegistrationAttribute(TypeInfo iface) =>
        iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0;

    /// <summary>
    /// Registers the default <see cref="ICommandDispatcher"/> implementation
    /// (<see cref="CommandDispatcher"/>) if one has not already been added.
    /// </summary>
    /// <param name="services">
    /// The service collection to modify.
    /// </param>
    /// <returns>
    /// The updated service collection.
    /// </returns>
    public static IServiceCollection AddFrankCoreApplicationCqrsCommandDispatcher(this IServiceCollection services)
    {
        services.TryAddScoped<ICommandDispatcher, CommandDispatcher>();
        return services;
    }

    /// <summary>
    /// Registers CQRS command handlers discovered from the specified assemblies.
    ///
    /// <para>
    /// Discovery rules:
    /// <list type="bullet">
    ///   <item><description>
    ///   Interfaces must be generic <see cref="ICommandHandler{TCommand}"/> or
    ///   <see cref="ICommandHandler{TCommand, TResponse}"/> and decorated with
    ///   <see cref="RegistrationAttribute"/>.
    ///   </description></item>
    ///   <item><description>
    ///   Implementations must implement one of the above handler interfaces.
    ///   </description></item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// The <paramref name="configure"/> callback allows callers to customize
    /// discovery behavior, including additional filters or overrides.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to modify.
    /// </param>
    /// <param name="assembliesToSearch">
    /// The assemblies to scan for handler interfaces and implementations.
    /// </param>
    /// <param name="configure">
    /// Optional callback for customizing discovery options.
    /// </param>
    /// <returns>
    /// The updated service collection.
    /// </returns>
    public static IServiceCollection AddFrankCoreApplicationCqrsCommands(
        this IServiceCollection services,
        IEnumerable<Assembly>? assembliesToSearch = null,
        Action<DiscoveryOptions>? configure = null)
    {
        services.AddFrankCoreApplicationCqrsCommandDispatcher();

        if (assembliesToSearch is null || !assembliesToSearch.Any())
        {
            return services;
        }

        var options = new DiscoveryOptions();

        //
        // Interface discovery:
        // Must be ICommandHandler<> or ICommandHandler<,>
        // AND decorated with [Registration]
        //
        options.IncludeInterfaces(iface =>
            HasRegistrationAttribute(iface) &&
            iface.IsGenericType &&
            (
                iface.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
            ));

        //
        // Implementation discovery:
        // Any class implementing ICommandHandler<> or ICommandHandler<,>
        //
        options.IncludeImplementations(impl =>
            impl.ImplementedInterfaces.Any(i =>
                i.IsGenericType &&
                (
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
                )));

        configure?.Invoke(options);

        IEnumerable<Assembly> assemblies = [
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
