using Microsoft.AspNetCore.Routing;

namespace Frank.Api.Endpoints;

public static class EndpointRoutBuilderExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        EndpointDiscovery.MapEndpoints(app);
        return app;
    }
}
