#nullable enable
using Microsoft.AspNetCore.Builder;

namespace Frank.Identity.Api.Middleware.Authorization;

/// <summary>
/// Provides extension methods for registering the Identity API’s authorization
/// middleware into the ASP.NET Core request pipeline.
/// <para>
/// This extension installs <see cref="RequireAuthenticatedUserMiddleware"/>,
/// which enforces the rule that all Identity API endpoints require an
/// authenticated user unless explicitly marked with <c>[AllowAnonymous]</c>.
/// </para>
/// </summary>
/// <remarks>
/// This middleware extension aligns with the Identity subsystem’s purity and
/// safety guarantees:
/// <list type="bullet">
/// <item><description>Authorization enforcement is centralized and predictable.</description></item>
/// <item><description>Anonymous access is opt‑in and explicit.</description></item>
/// <item><description>No domain logic is embedded in middleware registration.</description></item>
/// </list>
/// </remarks>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the Identity API authorization middleware to the application pipeline.
    /// <para>
    /// This middleware ensures that:
    /// <list type="bullet">
    /// <item><description>Authenticated users are required for all endpoints unless explicitly marked anonymous.</description></item>
    /// <item><description>Unauthorized requests fail fast with HTTP 401.</description></item>
    /// <item><description>Authorization behavior remains consistent across the entire Identity API surface.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="app">The application builder used to configure the middleware pipeline.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance for fluent chaining.</returns>
    public static IApplicationBuilder UseFrankIdentityApiMiddlewareAuthorization(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<RequireAuthenticatedUserMiddleware>();
    }
}
