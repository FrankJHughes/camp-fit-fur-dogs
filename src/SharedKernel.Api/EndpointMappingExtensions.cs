using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace SharedKernel.Api;

public static class EndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapDiscoveredEndpoints(this IEndpointRouteBuilder app)
    {
        EndpointDiscovery.MapDiscoveredEndpoints(app);
        return app;
    }
}
