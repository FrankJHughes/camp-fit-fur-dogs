#nullable enable
using Frank.Core.Api.Middleware;
using Frank.Identity.Api.Authentication;
using Frank.Identity.Api.Authorization;
using Frank.Identity.Application;
using Frank.Identity.EntityFrameworkCore;
using Frank.Identity.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Api.Platform;

/// <summary>
/// Provides extension methods for registering all Identity‑related platform
/// services required by the API surface hosted in this assembly.
/// <para>
/// This method composes the Identity subsystem’s service graph, wiring together:
/// authentication, authorization, application pipelines, EF Core persistence,
/// infrastructure services, and shared API middleware.
/// </para>
/// </summary>
/// <remarks>
/// Although this platform configuration lives in the Identity API assembly,
/// it is not limited to identity endpoints.
/// It resides here because it depends on Identity abstractions such as:
/// <list type="bullet">
/// <item><description>Authentication configuration for the Identity session scheme.</description></item>
/// <item><description>Authorization policies based on Identity’s user model.</description></item>
/// <item><description>Application pipelines for identity operations.</description></item>
/// <item><description>EF Core persistence for identity domain entities.</description></item>
/// <item><description>Infrastructure services required by the Identity subsystem.</description></item>
/// </list>
/// This method acts as the **service‑composition root** for the Identity API.
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Identity API platform services into the DI container.
    /// <para>
    /// The following subsystems are added:
    /// <list type="bullet">
    /// <item><description><b>Authentication</b> — Identity session scheme configuration.</description></item>
    /// <item><description><b>Authorization</b> — Identity authorization policies.</description></item>
    /// <item><description><b>Application</b> — Identity application pipelines.</description></item>
    /// <item><description><b>Entity Framework Core</b> — Identity persistence layer.</description></item>
    /// <item><description><b>Infrastructure</b> — Token hashing, correlation, and other identity services.</description></item>
    /// <item><description><b>Core API Middleware</b> — Shared middleware used across the API surface.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration root.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
    public static IServiceCollection AddFrankIdentityApiPlatform(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddFrankIdentityApiAuthentication(configuration)
            .AddFrankIdentityApiAuthorization()
            .AddFrankIdentityApplication()
            .AddFrankIdentityEntityFrmeworkCore(configuration)
            .AddFrankIdentityInfrastructure()
            .AddFrankCoreApiMiddleware();
    }
}
