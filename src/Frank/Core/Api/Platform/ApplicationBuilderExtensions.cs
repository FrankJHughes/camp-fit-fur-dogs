#nullable enable
using Frank.Core.Api.Platform.Swagger;
using Frank.Core.Api.Middleware.Cors;
using Frank.Core.Api.Middleware.Exceptions;
using Frank.Core.Api.Middleware.Observations;
using Microsoft.AspNetCore.Builder;
using Frank.Core.Api.Platform.Logging;

namespace Frank.Core.Api.Platform;

/// <summary>
/// Provides extension methods for composing the full Frank.Core API platform
/// middleware pipeline.
/// <para>
/// This subsystem defines the top‑level ordering of all cross‑cutting platform
/// middleware, ensuring consistent behavior across environments and vertical
/// slices.
/// The pipeline is intentionally structured into phases covering logging,
/// exception handling, observability, routing, CORS, origin logging, and
/// development‑only Swagger exposure.
/// </para>
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the Frank.Core API platform pipeline using a deterministic,
    /// phase‑driven middleware ordering model.
    /// <para>
    /// The pipeline consists of four phases:
    /// <list type="number">
    /// <item>
    /// <description>
    /// <b>Global logging + exception boundary</b> — enables HTTP logging in
    /// development and establishes the global exception handler.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <b>Observability</b> — creates and populates the request‑level observation
    /// context for correlation and tracing.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <b>Routing + CORS + origin logging</b> — activates endpoint routing,
    /// applies the platform CORS policy, and logs inbound origin information.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <b>Swagger (pre‑auth)</b> — exposes the OpenAPI document only in
    /// development environments.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// This method ensures that all cross‑cutting concerns are applied in the
    /// correct order, providing a secure, observable, and developer‑friendly API
    /// platform.
    /// </para>
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
    /// <returns>
    /// The same <see cref="WebApplication"/> instance, enabling fluent chaining.
    /// </returns>
    public static WebApplication UseFrankCoreApiPlatform(this WebApplication app)
    {
        // Phase 1 — Global logging + exception boundary
        app.UseFrankCoreApiPlatformLogging();
        app.UseFrankCoreApiMiddlewareExceptions();

        // Phase 2 — Observability (request-level)
        app.UseFrankCoreApiMiddlewareObservations();

        // Phase 3 — Routing + CORS + origin logging
        app.UseRouting();
        app.UseCors();
        app.UseFrankCoreApiMiddlewareOriginLogging();

        // Phase 4 — Swagger (pre-auth)
        app.UseFrankCoreApiPlatformSwagger();

        return app;
    }
}
