using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Queries;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application.Dogs;

/// <summary>
/// Provides extension methods for registering all CQRS components belonging to
/// the Dogs vertical slice.
/// <para>
/// This extension centralizes the discovery and registration of command and
/// query handlers, validators, and related application‑layer components defined
/// within the <c>CampFitFurDogs.Application.Dogs</c> namespace.
/// </para>
/// <para>
/// Registration is performed using the Frank.Core discovery conventions, which
/// scan the assembly for types implementing CQRS interfaces and automatically
/// bind them to the dependency injection container.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Dogs vertical‑slice CQRS components into the provided
    /// <see cref="IServiceCollection"/>.
    /// <para>
    /// This includes:
    /// <list type="bullet">
    /// <item><description>Command handlers implementing <see cref="ICommandHandler{TCommand}"/> or <see cref="ICommandHandler{TCommand, TResult}"/>.</description></item>
    /// <item><description>Query handlers implementing <see cref="IQueryHandler{TQuery, TResult}"/>.</description></item>
    /// <item><description>Validators discovered via FluentValidation conventions.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Discovery is restricted to types whose namespaces begin with the Dogs
    /// application namespace, ensuring that only slice‑specific components are
    /// registered.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The dependency injection container to which Dogs CQRS components will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    public static IServiceCollection AddApplicationDogs(
        this IServiceCollection services)
    {
        static DiscoveryOptions updateOptions(DiscoveryOptions options) =>
            options.IncludeInterfaces(t =>
                !string.IsNullOrWhiteSpace(t.Namespace) &&
                t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services

            // Register command handlers for the Dogs vertical slice
            .AddFrankCoreApplicationCqrsCommands(
                [typeof(AssemblyMarker).Assembly],
                discoveryOptions => updateOptions(discoveryOptions))

            // Register query handlers for the Dogs vertical slice
            .AddFrankCoreApplicationCqrsQueries(
                [typeof(AssemblyMarker).Assembly],
                discoveryOptions => updateOptions(discoveryOptions));
    }
}
