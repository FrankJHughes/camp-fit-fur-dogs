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
/// for registering its own route(s) using the provided
/// <see cref="IEndpointRouteBuilder"/>.
/// </para>
///
/// <para>
/// The <see cref="RegistrationAttribute"/> ensures that all endpoints are
/// automatically registered into the dependency injection container with a
/// singleton lifetime, allowing the application to discover and map them during
/// startup.
/// </para>
/// </summary>
[Registration(ServiceLifetime.Singleton)]
public interface IEndpoint
{
    /// <summary>
    /// Maps the endpoint into the application's routing pipeline.
    ///
    /// <para>
    /// Implementations use the supplied <see cref="IEndpointRouteBuilder"/> to
    /// define routes, configure metadata, and attach handlers. This method is
    /// typically invoked during application startup when all endpoints are
    /// registered and ready to be mapped.
    /// </para>
    /// </summary>
    /// <param name="app">
    /// The route builder used to configure and register endpoint routes.
    /// </param>
    void Map(IEndpointRouteBuilder app);
}
