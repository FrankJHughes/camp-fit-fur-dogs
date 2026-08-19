using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CampFitFurDogs.Api.Endpoints.Dogs;

/// <summary>
/// Handles HTTP DELETE requests for removing a dog owned by the currently
/// authenticated user.
/// <para>
/// This endpoint ensures ownership, verifies the dog exists, and dispatches a
/// removal command through the application command pipeline.
/// </para>
/// </summary>
public class RemoveDogEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the <c>/dogs/{id}</c> route to the remove‑dog operation.
    /// <para>
    /// The <c>/api</c> prefix is applied automatically by the API group created
    /// in <c>MapRegisteredApiEndpoints("/api")</c>.
    /// </para>
    /// </summary>
    /// <param name="api">The API route group created by Frank.Core.</param>
    public void Map(RouteGroupBuilder api)
    {
        api.MapDelete("/dogs/{id:guid}", async (
            Guid id,
            [FromServices] ICurrentUser currentUser,
            [FromServices] ICommandDispatcher commandDispatcher,
            [FromServices] IQueryDispatcher queryDispatcher) =>
        {
            var userId = currentUser.Id!.Value;

            // Ensure the dog exists and belongs to the current user
            var query = new GetDogQuery(DogId: id, OwnerId: userId);
            var response = await queryDispatcher.DispatchAsync(query, CancellationToken.None);
            if (response is null)
            {
                return Results.NotFound();
            }

            // Remove the dog
            var command = new RemoveDogCommand(DogId: id, OwnerId: userId);
            await commandDispatcher.DispatchAsync(command, CancellationToken.None);

            return Results.NoContent();
        });
    }
}
