using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Frank.Api;

namespace Frank.Testing.Endpoints;

public sealed class HealthCheckEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/__test__/health", () =>
        {
            return Results.Ok(new { status = "ok" });
        });
    }
}
