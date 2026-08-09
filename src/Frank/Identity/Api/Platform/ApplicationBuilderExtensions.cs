#nullable enable
using Frank.Identity.Api.Middleware.Authorization;
using Frank.Identity.Api.Middleware.Observations;
using Frank.Identity.Api.Middleware.Sessions;
using Microsoft.AspNetCore.Builder;

namespace Frank.Identity.Api.Platform;

/// <summary>
/// Provides extension methods for configuring the complete Identity API platform
/// middleware pipeline.
/// <para>
/// This extension assembles all cross‑cutting middleware layers — Observations,
/// Session Validation, Authentication, Authorization — into the correct order
/// required for safe and predictable request processing.
/// </para>
/// </summary>
/// <remarks>
/// Although this pipeline lives in the Identity API assembly, it is not limited
/// to identity endpoints.
/// It resides here because it depends on Identity abstractions such as:
/// <list type="bullet">
/// <item><description><see cref="Frank.Identity.Application.Abstractions.Users.ICurrentUser"/></description></item>
/// <item><description><see cref="Frank.Identity.Application.Abstractions.Sessions.ISessionTokenGenerator"/></description></item>
/// <item><description><see cref="Frank.Identity.Application.Abstractions.Sessions.GetSession.IGetSessionReader"/></description></item>
/// <item><description><see cref="Frank.Identity.Application.Abstractions.Users.GetUserById.IGetUserByIdReader"/></description></item>
/// </list>
/// The ordering of middleware is intentional:
/// <list type="number">
/// <item><description>Observations — capture full request lifecycle.</description></item>
/// <item><description>Authentication — enable ASP.NET Core auth infrastructure.</description></item>
/// <item><description>Forwarded headers — support reverse proxies.</description></item>
/// <item><description>Session Validation — attach authenticated principals.</description></item>
/// <item><description>Authorization — enforce authenticated access.</description></item>
/// <item><description>Identity Authorization — enforce Identity purity rules.</description></item>
/// </list>
/// </remarks>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the full Identity API platform middleware pipeline.
    /// <para>
    /// This method installs:
    /// <list type="bullet">
    /// <item><description>Observations middleware</description></item>
    /// <item><description>ASP.NET Core authentication</description></item>
    /// <item><description>Forwarded headers support</description></item>
    /// <item><description>Session validation middleware</description></item>
    /// <item><description>ASP.NET Core authorization</description></item>
    /// <item><description>Identity authorization middleware</description></item>
    /// </list>
    /// in the correct order required for safe and predictable request processing.
    /// </para>
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
    /// <returns>The same <see cref="WebApplication"/> instance for fluent chaining.</returns>
    public static WebApplication UseFrankIdentityApiPlatform(this WebApplication app)
    {
        app.UseFrankIdentityApiMiddlewareObservations();
        app.UseAuthentication();
        app.UseForwardedHeaders();
        app.UseFrankIdentityApiMiddlewareSessionValidation();
        app.UseAuthorization();
        app.UseFrankIdentityApiMiddlewareAuthorization();

        return app;
    }
}
