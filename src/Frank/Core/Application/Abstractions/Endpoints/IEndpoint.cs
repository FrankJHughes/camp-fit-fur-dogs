using Frank.Core.Application.Registration;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Abstractions.Endpoints;

/// <summary>
/// Defines the contract for an application endpoint that can be mapped into the
/// ASP.NET Core routing pipeline.
///
/// <para>
/// Implementations of <see cref="IEndpoint"/> encapsulate the routing and
/// configuration required to expose HTTP endpoints. Each endpoint is responsible
/// for registering its own route(s) using the provided <see cref="RouteGroupBuilder"/>.
/// </para>
///
/// <para>
/// The <see cref="RegistrationAttribute"/> ensures that all endpoints are
/// automatically registered into the dependency injection container with a
/// singleton lifetime, allowing the application to discover and map them during
/// startup.
/// </para>
///
/// <para>
/// Endpoints are mapped into a unified API group created by
/// <c>MapRegisteredApiEndpoints("/api")</c>. Implementations must define routes
/// relative to that group (e.g., <c>"/dogs/{id}"</c> instead of <c>"/api/dogs/{id}"</c>).
/// </para>
/// </summary>
[Registration(ServiceLifetime.Singleton)]
public interface IEndpoint
{
    /// <summary>
    /// Maps the endpoint into the application's API route group.
    ///
    /// <para>
    /// Implementations use the supplied <see cref="RouteGroupBuilder"/> to
    /// define routes, configure metadata, and attach handlers. This method is
    /// invoked during application startup after all endpoints have been
    /// discovered and the API group has been created.
    /// </para>
    ///
    /// <para>
    /// Routes must be defined relative to the provided group. The API prefix
    /// (e.g., <c>"/api"</c>) is applied automatically by the application.
    /// </para>
    /// </summary>
    /// <param name="api">
    /// The API route group created by <c>MapRegisteredApiEndpoints</c>.
    /// </param>
    void Map(RouteGroupBuilder api);
}
