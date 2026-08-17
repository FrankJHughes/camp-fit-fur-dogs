using FluentValidation;
using Frank.Identity.Application.Callback;
using Frank.Identity.Application.Sessions;
using Frank.Identity.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application;

/// <summary>
/// Provides extension methods for registering the entire
/// <c>Frank.Identity.Application</c> module into the dependency injection
/// container.
/// <para>
/// This method composes all Identity application subsystems:
/// </para>
/// <list type="bullet">
/// <item><description>Session management (token generation, retrieval, revocation)</description></item>
/// <item><description>User management (resolution, creation, queries)</description></item>
/// <item><description>OIDC callback pipelines (external identity acquisition + save pipeline)</description></item>
/// <item><description>FluentValidation validators for all application‑layer requests</description></item>
/// </list>
/// <para>
/// By centralizing registration, this extension ensures that the Identity
/// application layer is wired consistently and predictably across all hosting
/// environments.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Identity application‑layer services, including sessions,
    /// users, callback pipelines, and validators.
    /// <para>
    /// This is the primary entry point for wiring the Identity application
    /// module into an API or worker service.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which the Identity application services will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, allowing fluent chaining.
    /// </returns>
    public static IServiceCollection AddFrankIdentityApplication(this IServiceCollection services)
    {
        return services

            // Session subsystem (token generation, retrieval, revocation)
            .AddFrankIdentityApplicationSessions()

            // User subsystem (resolution, creation, queries)
            .AddFrankIdentityApplicationUsers()

            // OIDC callback pipelines (external identity → internal identity → session)
            .AddFrankIdentityApplicationCallback()

            // Register all FluentValidation validators in this assembly
            .AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
    }
}
