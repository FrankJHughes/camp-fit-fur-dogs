using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Queries;
using Frank.Core.Application.Registration;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Users;

/// <summary>
/// Provides extension methods for registering all user‑related application
/// services into the dependency injection container.
/// <para>
/// This includes:
/// </para>
/// <list type="bullet">
/// <item><description>The <see cref="IUserResolver"/> used during authentication and onboarding</description></item>
/// <item><description>All CQRS command handlers in the Users assembly</description></item>
/// <item><description>All CQRS query handlers in the Users assembly</description></item>
/// </list>
/// <para>
/// Handler discovery is restricted to interfaces within the
/// <c>Frank.Identity.Application.Users</c> namespace (and its sub‑namespaces),
/// ensuring that only user‑related handlers are registered.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all user‑related application services, including the
    /// <see cref="IUserResolver"/> and all CQRS command/query handlers found in
    /// the Users assembly.
    /// <para>
    /// This method configures <see cref="DiscoveryOptions"/> so that only
    /// interfaces belonging to the <c>Frank.Identity.Application.Users</c>
    /// namespace are included during handler discovery.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which the user services will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, allowing fluent
    /// chaining of registration calls.
    /// </returns>
    public static IServiceCollection AddFrankIdentityApplicationUsers(this IServiceCollection services)
    {
        static DiscoveryOptions updateOptions(DiscoveryOptions options) => options.IncludeInterfaces(t =>
            !string.IsNullOrWhiteSpace(t.Namespace) &&
            t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services
            // User resolution (OIDC → internal user)
            .AddScoped<IUserResolver, UserResolver>()

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
