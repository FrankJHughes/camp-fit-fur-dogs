#nullable enable
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Platform.Swagger;

/// <summary>
/// Provides extension methods for registering platform-level Swagger and OpenAPI
/// services for the Frank.Core API.
/// <para>
/// This subsystem registers the OpenAPI generation infrastructure so that the
/// application can expose its API specification during development.
/// The corresponding application pipeline extension determines when the
/// OpenAPI endpoint is actually mapped.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Frank.Core API's OpenAPI generation services.
    /// <para>
    /// This method adds the OpenAPI document generator via <c>AddOpenApi()</c>,
    /// enabling the application to produce an OpenAPI specification that can be
    /// exposed conditionally at runtime (typically only in Development).
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    public static IServiceCollection AddFrankCoreApiPlatformSwagger(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }
}
