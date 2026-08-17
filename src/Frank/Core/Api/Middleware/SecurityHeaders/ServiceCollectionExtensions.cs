#nullable enable
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Middleware.SecurityHeaders;

/// <summary>
/// Provides extension methods for registering the security‑header middleware
/// components used by the Frank.Core API.
/// <para>
/// This extension ensures that the <see cref="SecurityHeadersMiddleware"/> is
/// available for injection into the ASP.NET Core pipeline, allowing hardened
/// OWASP‑aligned security headers to be applied to all outgoing responses.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="SecurityHeadersMiddleware"/> with the service
    /// collection so it can be used within the ASP.NET Core middleware pipeline.
    /// <para>
    /// The middleware applies a strict set of modern security headers, including
    /// CSP, clickjacking protection, MIME‑sniffing prevention, and cross‑origin
    /// isolation policies.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which the security‑header middleware will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent configuration.
    /// </returns>
    public static IServiceCollection AddFrankCoreApiSecurityHeaders(this IServiceCollection services)
    {
        services.AddTransient<SecurityHeadersMiddleware>();
        return services;
    }
}
