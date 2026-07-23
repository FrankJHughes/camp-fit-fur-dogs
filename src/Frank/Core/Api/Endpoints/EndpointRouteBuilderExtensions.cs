using Frank.Core.Application.Abstractions.Endpoints;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Endpoints;

public static class EndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapFrankCoreApiEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.ServiceProvider.GetServices<IEndpoint>();

        foreach (var endpoint in endpoints)
            endpoint.Map(app);

        return app;
    }
}
