using Frank.Abstractions;
using Frank.Abstractions.Observations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.TestUtilities.Endpoints;

public sealed class TraceEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/__test__/trace", (IObservationSink trace, IRequestObservationContext ctx) =>
        {
            trace.Emit(
                "test_trace_event",   // name
                "test",               // category
                "trace emitted",      // message
                null,                 // data
                ctx                   // context
            );

            return Results.Ok(new { message = "trace emitted" });
        })
        .AllowAnonymous();
    }
}
