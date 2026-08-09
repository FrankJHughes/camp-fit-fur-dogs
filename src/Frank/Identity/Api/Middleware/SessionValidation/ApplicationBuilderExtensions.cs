#nullable enable
using Microsoft.AspNetCore.Builder;

namespace Frank.Identity.Api.Middleware.Sessions;

/// <summary>
/// Provides extension methods for registering the Identity API’s session‑validation
/// middleware into the ASP.NET Core request pipeline.
/// <para>
/// This extension installs <see cref="SessionValidationMiddleware"/>, which performs
/// the actual session‑cookie validation, domain‑invariant enforcement, and principal
/// attachment for the “Session” authentication scheme.
/// </para>
/// </summary>
/// <remarks>
/// Although this middleware lives in the Identity API assembly, it is not limited
/// to identity endpoints.
/// It resides here because it depends on Identity abstractions such as
/// <see cref="Frank.Identity.Application.Abstractions.Sessions.ISessionTokenGenerator"/>,
/// <see cref="Frank.Identity.Application.Abstractions.Sessions.GetSession.IGetSessionReader"/>,
/// and <see cref="Frank.Identity.Application.Abstractions.Users.GetUserById.IGetUserByIdReader"/>.
/// </remarks>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the Identity session‑validation middleware to the application pipeline.
    /// <para>
    /// This middleware ensures that:
    /// <list type="bullet">
    /// <item><description>Session cookies are hashed and validated.</description></item>
    /// <item><description>Revoked or expired sessions are rejected.</description></item>
    /// <item><description>Authenticated principals are attached to <see cref="HttpContext.User"/>.</description></item>
    /// <item><description>Downstream authentication handlers (e.g., “Session”) can rely on a populated principal.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="app">The application builder used to configure the middleware pipeline.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance for fluent chaining.</returns>
    public static IApplicationBuilder UseFrankIdentityApiMiddlewareSessionValidation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SessionValidationMiddleware>();
    }
}
