#nullable enable

using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Application.Abstractions.Observations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.TestUtilities.Endpoints;

public sealed class CorrelationEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder api)
    {
        api.MapGet("/__test__/correlation", (IRequestObservationContext observabilityContext) =>
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
