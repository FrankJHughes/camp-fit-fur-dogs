using Microsoft.AspNetCore.Routing;

namespace Frank.Api;

public static class EndpointDiscoveryExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        EndpointDiscovery.MapEndpoints(app);
        return app;
    }
}
