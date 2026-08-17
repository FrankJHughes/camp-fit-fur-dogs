#nullable enable
using System.Net;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Frank.Identity.Api.Middleware.Authorization;

/// <summary>
/// Middleware that enforces the Identity API’s authorization baseline:
/// all endpoints require an authenticated user unless explicitly marked
/// with <see cref="IAllowAnonymous"/>.
/// <para>
/// This middleware provides a lightweight authorization guard that runs
/// before endpoint execution, ensuring predictable behavior across the
/// Identity API surface.
/// </para>
/// </summary>
/// <remarks>
/// This middleware aligns with Identity purity and safety rules:
/// <list type="bullet">
/// <item><description>Anonymous access is allowed only when explicitly declared.</description></item>
/// <item><description>No domain logic is embedded in authorization enforcement.</description></item>
/// <item><description>Authentication state is resolved exclusively through <see cref="ICurrentUser"/>.</description></item>
/// <item><description>Unauthorized requests fail fast with HTTP 401.</description></item>
/// </list>
/// </remarks>
public sealed class RequireAuthenticatedUserMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the middleware.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public RequireAuthenticatedUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Executes authorization enforcement for the current request.
    /// <para>
    /// The flow is:
    /// <list type="number">
    /// <item><description>Check if the endpoint allows anonymous access.</description></item>
    /// <item><description>If anonymous, continue the pipeline.</description></item>
    /// <item><description>If not anonymous, verify <see cref="ICurrentUser.IsAuthenticated"/>.</description></item>
    /// <item><description>If unauthenticated, return HTTP 401.</description></item>
    /// <item><description>If authenticated, continue the pipeline.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="currentUser">The resolved identity of the current user.</param>
    public async Task InvokeAsync(HttpContext httpContext, ICurrentUser currentUser)
    {
        // Allow anonymous endpoints (e.g., login, health, registration)
        var endpoint = httpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            await _next(httpContext);
            return;
        }

        // Enforce authentication
        if (!currentUser.IsAuthenticated)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        await _next(httpContext);
    }
}
