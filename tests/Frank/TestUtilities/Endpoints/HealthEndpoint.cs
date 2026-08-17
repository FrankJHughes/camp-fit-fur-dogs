#nullable enable

using Frank.Core.Application.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.TestUtilities.Endpoints;

public sealed class HealthEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder api)
    {
        api.MapGet("/__test__/health", () =>
        {
            return Results.Ok(new { status = "ok" });
        })
        .AllowAnonymous();
    }
}
