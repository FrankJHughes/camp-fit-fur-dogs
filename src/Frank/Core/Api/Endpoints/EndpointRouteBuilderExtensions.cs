using Frank.Core.Application.Abstractions.Endpoints;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Endpoints;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapRegisteredApiEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.ServiceProvider
            .GetServices<IEndpoint>()
            .DistinctBy(endpoint =>
                endpoint.GetType().FullName)
            .ToList();

        foreach (var endpoint in endpoints)
        {
            endpoint.Map(app);
        }

        return app;
    }
}
