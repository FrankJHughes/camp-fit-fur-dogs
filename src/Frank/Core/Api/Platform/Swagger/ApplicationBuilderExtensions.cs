#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Api.Platform.Swagger;

/// <summary>
/// Provides extension methods for enabling platform-level Swagger and OpenAPI
/// features for the Frank.Core API.
/// <para>
/// This subsystem conditionally exposes the OpenAPI endpoint during development,
/// ensuring that API documentation is available to developers without exposing
/// it in production environments.
/// </para>
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Enables the Frank.Core API's OpenAPI endpoint when running in the
    /// Development environment.
    /// <para>
    /// When <see cref="IHostEnvironment.IsDevelopment"/> is <c>true</c>, this method:
    /// <list type="bullet">
    /// <item><description>Maps the OpenAPI document via <c>MapOpenApi()</c>.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// In non-development environments, the OpenAPI endpoint is not exposed,
    /// ensuring production deployments remain secure and minimal.
    /// </para>
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
    /// <returns>
    /// The same <see cref="WebApplication"/> instance, enabling fluent configuration.
    /// </returns>
    public static WebApplication UseFrankCoreApiPlatformSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        return app;
    }
}
