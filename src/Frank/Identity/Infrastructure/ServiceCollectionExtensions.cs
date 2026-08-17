using Frank.Identity.Infrastructure.Auth0;
using Frank.Identity.Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Infrastructure;

/// <summary>
/// Provides extension methods for registering all infrastructure components
/// required by the Identity subsystem.
/// <para>
/// This method serves as the top‑level entry point for wiring up Identity
/// infrastructure, delegating to feature‑specific registration methods such as:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="Users.ServiceCollectionExtensions.AddFrankIdentityInfrastructureUsers"/></description></item>
/// <item><description><see cref="Auth0.ServiceCollectionExtensions.AddFrankIdentityInfrastructureAuth0"/></description></item>
/// </list>
/// <para>
/// By centralizing these registrations, the application startup code remains
/// clean, predictable, and aligned with vertical‑slice boundaries.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Identity infrastructure services, including user accessors
    /// and Auth0‑based OIDC components.
    /// <para>
    /// This method composes the full Identity infrastructure layer by invoking:
    /// </para>
    /// <list type="bullet">
    /// <item><description>
    /// <c>AddFrankIdentityInfrastructureUsers()</c> — registers <see cref="ICurrentUser"/>
    /// </description></item>
    /// <item><description>
    /// <c>AddFrankIdentityInfrastructureAuth0()</c> — registers OIDC token, validation,
    /// and userinfo clients, plus OIDC settings and audit logging
    /// </description></item>
    /// </list>
    /// <para>
    /// The returned <see cref="IServiceCollection"/> enables fluent chaining during
    /// application startup.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which Identity infrastructure services will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    public static IServiceCollection AddFrankIdentityInfrastructure(this IServiceCollection services)
    {
        return services
            .AddFrankIdentityInfrastructureUsers()
            .AddFrankIdentityInfrastructureAuth0();
    }
}
