using Frank.Core.Application.Abstractions.Endpoints;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Endpoints;

/// <summary>
/// Provides extension methods for mapping API endpoints that have been
/// registered in the dependency injection container.
/// <para>
/// This enables vertical‑slice endpoint discovery: any implementation of
/// <see cref="IEndpoint"/> registered in DI will be automatically mapped
/// when the application starts.
/// </para>
/// <para>
/// Endpoint classes are expected to implement <see cref="IEndpoint"/> and
/// define their own routing via the <c>Map</c> method.
/// This extension ensures all such endpoints are invoked exactly once,
/// avoiding duplicate registrations through <c>DistinctBy</c>.
/// </para>
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Discovers all registered <see cref="IEndpoint"/> implementations from
    /// the service provider and maps them into the application's routing
    /// pipeline.
    /// <para>
    /// Endpoints are deduplicated by their concrete type name to prevent
    /// accidental double‑registration when multiple slices reference the same
    /// endpoint type.
    /// </para>
    /// </summary>
    /// <param name="app">
    /// The <see cref="IEndpointRouteBuilder"/> used to configure API routing.
    /// </param>
    /// <returns>
    /// The same <see cref="IEndpointRouteBuilder"/> instance, enabling fluent
    /// configuration.
    /// </returns>
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
