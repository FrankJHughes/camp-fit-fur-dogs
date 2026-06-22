using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

using Frank.Api;

namespace Frank.Testing.Endpoints;

public sealed class ThrowEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/__test__/throw", () =>
        {
            throw new InvalidOperationException("Test exception");
        });
    }
}
