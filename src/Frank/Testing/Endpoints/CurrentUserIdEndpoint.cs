#nullable enable

using System.Security.Claims;
using Frank.Core.Application.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.Testing.Endpoints;

/// <summary>
/// A test‑only endpoint that exposes the current authenticated user's
/// <c>NameIdentifier</c> claim (typically the OIDC <c>sub</c>).
/// <para>
/// This endpoint is used exclusively in the testing harness to verify that
/// authentication flows, claim injection, and test client mutation behave
/// correctly. It is not intended for production use.
/// </para>
/// </summary>
public sealed class CurrentUserIdEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the <c>/__test__/current-user-id</c> endpoint, returning the current
    /// user's identifier as JSON.
    /// <para>
    /// If no user is authenticated or the claim is missing, the returned
    /// <c>userId</c> value will be <c>null</c>.
    /// </para>
    /// </summary>
    /// <param name="api">The API route group created by Frank.Core.</param>
    public void Map(RouteGroupBuilder api)
    {
        api.MapGet("/__test__/current-user-id", (HttpContext http) =>
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Results.Json(new { userId });
        });
    }
}
