using Frank.Abstractions;
using Frank.Abstractions.Observations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.TestUtilities.Endpoints;

public sealed class MetricsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/__test__/metrics", (IMetrics metrics, IRequestObservationContext ctx) =>
        {
            metrics.Increment("test_metric", 1, ctx);
            return Results.Ok(new { message = "metric incremented" });
        })
        .AllowAnonymous();
    }
}
