using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Infrastructure.AuditLogging;
using Frank.Identity.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Infrastructure.Auth0;

/// <summary>
/// Provides extension methods for registering all Auth0‑based OIDC infrastructure
/// components used by the Identity subsystem.
/// <para>
/// This includes configuration binding for <see cref="OidcSettings"/>, audit
/// logging, and all Auth0 OIDC clients (token exchange, token validation, and
/// userinfo retrieval).
/// </para>
/// <para>
/// The method serves as the single entry point for wiring up Auth0 integration
/// during application startup.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Auth0 OIDC infrastructure services, including configuration,
    /// audit logging, and all OIDC client implementations.
    /// <para>
    /// The following components are added:
    /// </para>
    /// <list type="bullet">
    /// <item><description>
    /// <see cref="OidcSettings"/> bound from <c>Identity:Oidc</c> configuration
    /// </description></item>
    /// <item><description>
    /// <see cref="IAuditLogger"/> via <c>AddFrankIdentityInfrastructureAuditLogging()</c>
    /// </description></item>
    /// <item><description>
    /// <see cref="IOidcUserInfoClient"/> → <see cref="Auth0OidcUserInfoClient"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="IOidcTokenClient"/> → <see cref="Auth0OidcTokenClient"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="IOidcTokenValidator"/> → <see cref="Auth0OidcTokenValidator"/>
    /// </description></item>
    /// </list>
    /// <para>
    /// All OIDC clients are registered as <c>Transient</c> because they are
    /// stateless and rely on externally managed <see cref="HttpClient"/> instances.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which Auth0 OIDC infrastructure services will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    public static IServiceCollection AddFrankIdentityInfrastructureAuth0(this IServiceCollection services)
    {
        services
            .AddOptions<OidcSettings>()
            .BindConfiguration("Identity:Oidc")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services

            .AddFrankIdentityInfrastructureAuditLogging()

            .AddTransient<IOidcUserInfoClient, Auth0OidcUserInfoClient>()
            .AddTransient<IOidcTokenClient, Auth0OidcTokenClient>()
            .AddTransient<IOidcTokenValidator, Auth0OidcTokenValidator>();
    }
}
