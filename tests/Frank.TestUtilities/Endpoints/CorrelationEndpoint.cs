using Frank.Abstractions.Observability;
using Frank.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.TestUtilities.Endpoints;

public sealed class CorrelationEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/__test__/correlation", (IObservabilityContext observabilityContext) =>
        {
            var correlationId = observabilityContext.CorrelationId;

            return Results.Json(new
            {
                correlationId
            });
        });
    }
}
