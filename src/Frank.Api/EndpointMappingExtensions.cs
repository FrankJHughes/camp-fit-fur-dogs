using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Frank.Api;

public static class EndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        EndpointDiscovery.MapEndpoints(app);
        return app;
    }
}
