using CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Endpoints.Dogs;

/// <summary>
/// Handles HTTP GET requests for retrieving all dogs owned by the currently
/// authenticated user.
/// <para>
/// This endpoint queries the dog list for the current user and returns a
/// collection of lightweight dog summaries suitable for dashboards and list views.
/// </para>
/// </summary>
public class ListDogsByCurrentUserEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the <c>/dogs</c> route to the list‑dogs operation.
    /// <para>
    /// The <c>/api</c> prefix is applied automatically by the API group created
    /// in <c>MapRegisteredApiEndpoints("/api")</c>.
    /// </para>
    /// </summary>
    /// <param name="api">The API route group created by Frank.Core.</param>
    public void Map(RouteGroupBuilder api)
    {
        api.MapGet("/dogs", async (
            [FromServices] ICurrentUser currentUser,
            [FromServices] IQueryDispatcher dispatcher) =>
        {
            var query = new ListDogsByOwnerQuery(currentUser.Id!.Value);
            var queryResponse = await dispatcher.DispatchAsync(query, CancellationToken.None);

            var endpointResponse = new ListDogsByCurrentUserEndpointResponse(
                Dogs: [.. queryResponse.Dogs.Select(dog =>
                    new GetDogSummaryEndpointResponse(
                        Id: dog.Id,
                        Name: dog.Name,
                        Breed: dog.Breed
                    ))]);

            return Results.Ok(endpointResponse);
        });
    }
}
