using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Api.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Frank.Core.Api.Routing.Validation;

namespace Frank.Core.Api.Endpoints;

/// <summary>
/// Provides extension methods for discovering and mapping all registered
/// <see cref="IEndpoint"/> implementations into a unified API route group.
/// <para>
/// The application supplies the API root path (e.g., <c>"/api"</c>) and owns
/// all API metadata such as tags, descriptions, and versioning.
/// </para>
/// <para>
/// Frank.Core applies cross‑cutting routing behaviors such as endpoint
/// filtering and request validation, discovers endpoints from the DI container,
/// and maps them into the created route group.
/// </para>
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Creates a top‑level API route group at the specified path, applies
    /// core‑owned routing behaviors, discovers all registered
    /// <see cref="IEndpoint"/> implementations, and maps them into the group.
    /// <para>
    /// The returned <see cref="RouteGroupBuilder"/> allows the application to
    /// apply API metadata such as tags and descriptions.
    /// </para>
    /// </summary>
    /// <param name="app">
    /// The application's root <see cref="IEndpointRouteBuilder"/>, used for
    /// endpoint discovery and group creation.
    /// </param>
    /// <param name="root">
    /// The API root path (e.g., <c>"/api"</c>) under which all endpoints will be
    /// grouped.
    /// </param>
    /// <returns>
    /// A <see cref="RouteGroupBuilder"/> representing the created API group,
    /// allowing the application to apply metadata.
    /// </returns>
    public static RouteGroupBuilder MapRegisteredApiEndpoints(
        this IEndpointRouteBuilder app,
        string root)
    {
        // Create the top-level API group
        var api = app.MapGroup(root);

        // Apply core-owned routing behaviors (Frank.Core.Routing)
        api.AddEndpointFiltering();
        api.AddRequestValidation();

        // Discover endpoints from DI
        var endpoints = app.ServiceProvider
            .GetServices<IEndpoint>()
            .DistinctBy(e => e.GetType().FullName)
            .ToList();

        // Map endpoints into the group
        foreach (var endpoint in endpoints)
        {
            endpoint.Map(api);
        }

        // App will apply metadata (tags, description, versioning)
        return api;
    }
}
