#nullable enable
using System.Net;
using Frank.Abstractions.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Frank.Infrastructure.Authorization;

public sealed class RequireAuthenticatedUserMiddleware
{
    private readonly RequestDelegate _next;

    public RequireAuthenticatedUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

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
