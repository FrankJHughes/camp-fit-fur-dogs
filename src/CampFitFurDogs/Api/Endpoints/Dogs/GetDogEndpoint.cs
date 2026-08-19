using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CampFitFurDogs.Api.Endpoints.Dogs;

/// <summary>
/// Handles HTTP GET requests for retrieving a single dog profile owned by the
/// currently authenticated user.
/// <para>
/// This endpoint ensures ownership, fetches the dog via the query pipeline,
/// and returns a structured response containing the dog's core profile details.
/// </para>
/// </summary>
public class GetDogEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the <c>/dogs/{id}</c> route to the get‑dog operation.
    /// <para>
    /// The <c>/api</c> prefix is applied automatically by the API group created
    /// in <c>MapRegisteredApiEndpoints("/api")</c>.
    /// </para>
    /// </summary>
    /// <param name="api">The API route group created by Frank.Core.</param>
    public void Map(RouteGroupBuilder api)
    {
        api.MapGet("/dogs/{id}", async (
            Guid id,
            [FromServices] ICurrentUser currentUser,
            [FromServices] IQueryDispatcher dispatcher) =>
        {
            var query = new GetDogQuery(id, currentUser.Id!.Value);
            var queryResponse = await dispatcher.DispatchAsync(query, CancellationToken.None);

            if (queryResponse is null)
            {
                return Results.NotFound();
            }

            var endpointResponse = new
            {
                Id = queryResponse.Id,
                OwnerId = queryResponse.OwnerId,
                Name = queryResponse.Name,
                Breed = queryResponse.Breed,
                Sex = queryResponse.Sex,
                DateOfBirth = queryResponse.DateOfBirth
            };

            return Results.Ok(endpointResponse);
        });
    }
}
