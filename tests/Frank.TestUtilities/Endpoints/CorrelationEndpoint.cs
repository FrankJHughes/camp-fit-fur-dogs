using Frank.Abstractions;
using Frank.Abstractions.Observations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.TestUtilities.Endpoints;

public sealed class CorrelationEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/__test__/correlation", (IRequestObservationContext observabilityContext) =>
        {
            var correlationId = observabilityContext.CorrelationId;

            return Results.Json(new
            {
                correlationId
            });
        })
        .AllowAnonymous();
    }
}
