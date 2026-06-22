using Frank.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.Api.Tests.TestEndpoints;

public sealed class CorrelationEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/__test__/correlation", (HttpContext http) =>
        {
            var correlationId = http.TraceIdentifier;

            return Results.Json(new
            {
                correlationId
            });
        });
    }
}
