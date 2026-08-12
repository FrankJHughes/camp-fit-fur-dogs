#nullable enable

using Frank.Core.Application.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.Testing.Endpoints;

/// <summary>
/// A lightweight test‑only health check endpoint used to verify that the test
/// host is running and able to serve requests.
/// <para>
/// This endpoint is intentionally simple and returns a static JSON payload
/// indicating that the test environment is operational.
/// It is not intended for production use.
/// </para>
/// </summary>
public sealed class HealthCheckEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the <c>/__test__/health</c> endpoint, returning a JSON object with
    /// a static <c>status = "ok"</c> value.
    /// <para>
    /// The endpoint is marked <c>AllowAnonymous()</c> so that tests can verify
    /// host availability without requiring authentication.
    /// </para>
    /// </summary>
    /// <param name="api">The API route group created by Frank.Core.</param>
    public void Map(RouteGroupBuilder api)
    {
        api.MapGet("/__test__/health", () =>
        {
            return Results.Ok(new { status = "ok" });
        })
        .AllowAnonymous();
    }
}
