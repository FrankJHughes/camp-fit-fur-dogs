using Frank.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Api.Endpoints;

public static class EndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapFrankEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.ServiceProvider.GetServices<IEndpoint>();

        foreach (var endpoint in endpoints)
            endpoint.Map(app);

        return app;
    }
}
