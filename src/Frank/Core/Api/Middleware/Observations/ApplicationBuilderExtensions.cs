#nullable enable
using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Api.Middleware.Observations;

/// <summary>
/// Provides extension methods for registering the inbound observation middleware
/// in the ASP.NET Core request pipeline.
/// <para>
/// The inbound middleware constructs an <see cref="IRequestObservationContext"/>
/// for each incoming HTTP request, capturing correlation identifiers, user
/// identity, environment metadata, clock information, and request details.
/// </para>
/// <para>
/// This extension ensures that every request entering the API has a fully
/// populated observation context available to downstream components, vertical
/// slices, and outbound HTTP handlers.
/// </para>
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="InboundObservationContextMiddleware"/> to the
    /// application's middleware pipeline.
    /// <para>
    /// This middleware should be placed early in the pipeline—typically before
    /// routing and endpoint execution—to ensure that all subsequent components
    /// have access to the initialized <see cref="IRequestObservationContext"/>.
    /// </para>
    /// </summary>
    /// <param name="app">
    /// The <see cref="IApplicationBuilder"/> used to configure the request pipeline.
    /// </param>
    /// <returns>
    /// The same <see cref="IApplicationBuilder"/> instance, enabling fluent
    /// configuration.
    /// </returns>
    public static IApplicationBuilder UseFrankCoreApiMiddlewareObservations(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<InboundObservationContextMiddleware>();
    }
}
