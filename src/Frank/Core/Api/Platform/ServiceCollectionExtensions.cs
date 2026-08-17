#nullable enable
using Frank.Core.Api.Middleware;
using Frank.Core.Api.Platform.Cors;
using Frank.Core.Api.Platform.Logging;
using Frank.Core.Api.Platform.Swagger;
using Frank.Core.Application;
using Frank.Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Platform;

/// <summary>
/// Provides extension methods for registering all platform-level services used by
/// the Frank.Core API.
/// <para>
/// This subsystem composes the platform’s cross‑cutting service registrations,
/// including CORS, logging, Swagger/OpenAPI, application services, infrastructure
/// services, and API middleware.
/// It acts as the unified entry point for configuring the API’s foundational
/// capabilities before the middleware pipeline is assembled.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the full Frank.Core API platform service set using a single,
    /// fluent extension method.
    /// <para>
    /// This method composes several subsystems:
    /// <list type="bullet">
    /// <item><description><b>Platform CORS</b> — configuration‑driven origin resolution and policy setup.</description></item>
    /// <item><description><b>Platform Logging</b> — HTTP logging infrastructure.</description></item>
    /// <item><description><b>Platform Swagger</b> — OpenAPI generation services.</description></item>
    /// <item><description><b>Application Layer</b> — domain‑level application services.</description></item>
    /// <item><description><b>Infrastructure Layer</b> — persistence, external services, and adapters.</description></item>
    /// <item><description><b>API Middleware</b> — cross‑cutting middleware subsystems (observations, exceptions, security headers, etc.).</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// This unified registration method ensures that all platform components are
    /// consistently configured and available before the API pipeline is built.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration used by platform subsystems.</param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    public static IServiceCollection AddFrankCoreApiPlatform(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddFrankCoreApiPlatformCors(configuration)
            .AddFrankCoreApiPlatformLogging()
            .AddFrankCoreApiPlatformSwagger()
            .AddFrankCoreApplication()
            .AddFrankCoreInfrastructure()
            .AddFrankCoreApiMiddleware();
    }
}
