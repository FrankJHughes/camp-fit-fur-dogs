#nullable enable
using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Api.Middleware.Cors;

/// <summary>
/// Provides extension methods for registering the
/// <see cref="OriginLoggingMiddleware"/> in the ASP.NET Core request pipeline.
/// <para>
/// This middleware logs detailed CORS activity, including origin evaluation,
/// preflight requests, and whether the request was allowed or blocked according
/// to the active CORS policy.
/// </para>
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="OriginLoggingMiddleware"/> to the application's
    /// middleware pipeline.
    /// <para>
    /// This should be placed early in the pipeline—typically immediately after
    /// CORS configuration—to ensure all CORS‑related requests are logged.
    /// </para>
    /// </summary>
    /// <param name="app">
    /// The <see cref="IApplicationBuilder"/> used to configure the request
    /// pipeline.
    /// </param>
    /// <returns>
    /// The same <see cref="IApplicationBuilder"/> instance, enabling fluent
    /// configuration.
    /// </returns>
    public static IApplicationBuilder UseFrankCoreApiMiddlewareOriginLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<OriginLoggingMiddleware>();
    }
}
