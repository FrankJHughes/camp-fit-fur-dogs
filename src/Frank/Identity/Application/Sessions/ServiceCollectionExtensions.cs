using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Queries;
using Frank.Core.Application.Registration;
using Frank.Identity.Application.Abstractions.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Sessions;

/// <summary>
/// Provides extension methods for registering all session‑related application
/// services into the dependency injection container.
/// <para>
/// This includes:
/// </para>
/// <list type="bullet">
/// <item><description>The session token generator</description></item>
/// <item><description>All CQRS command handlers in the Sessions assembly</description></item>
/// <item><description>All CQRS query handlers in the Sessions assembly</description></item>
/// </list>
/// <para>
/// The registration uses assembly scanning with <see cref="DiscoveryOptions"/>
/// to automatically discover and register handlers that belong to the
/// <c>Frank.Identity.Application.Sessions</c> namespace.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all session‑related application services, including the session
    /// token generator and all CQRS command/query handlers found in the Sessions
    /// assembly.
    /// <para>
    /// This method configures handler discovery so that only interfaces within
    /// the <c>Frank.Identity.Application.Sessions</c> namespace (and its
    /// sub‑namespaces) are included.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which the session services will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, allowing fluent
    /// chaining of registration calls.
    /// </returns>
    public static IServiceCollection AddFrankIdentityApplicationSessions(this IServiceCollection services)
    {
        static DiscoveryOptions updateOptions(DiscoveryOptions options) => options.IncludeInterfaces(t =>
            !string.IsNullOrWhiteSpace(t.Namespace) &&
            t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services

            // Session token generator
            .AddScoped<ISessionTokenGenerator, SessionTokenGenerator>()

            // Register all CQRS command handlers in this assembly
            .AddFrankCoreApplicationCqrsCommands(
                [typeof(AssemblyMarker).Assembly],
                discoveryOptions => updateOptions(discoveryOptions))

            // Register all CQRS query handlers in this assembly
            .AddFrankCoreApplicationCqrsQueries(
                [typeof(AssemblyMarker).Assembly],
                discoveryOptions => updateOptions(discoveryOptions));
    }
}
