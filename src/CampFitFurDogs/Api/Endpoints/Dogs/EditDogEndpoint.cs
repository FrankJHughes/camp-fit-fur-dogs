using CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.EditDog;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Endpoints.Dogs;

/// <summary>
/// Handles HTTP PUT requests for editing an existing dog profile.
/// <para>
/// This endpoint ensures that the authenticated user owns the dog being edited,
/// retrieves the current dog record, and applies updates through the application
/// command pipeline.
/// </para>
/// </summary>
public class EditDogEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the <c>/dogs/{id}</c> route to the edit‑dog operation.
    /// <para>
    /// The <c>/api</c> prefix is applied automatically by the API group created
    /// in <c>MapRegisteredApiEndpoints("/api")</c>.
    /// </para>
    /// </summary>
    /// <param name="api">The API route group created by Frank.Core.</param>
    public void Map(RouteGroupBuilder api)
    {
        api.MapPut("/dogs/{id}", async (
            [FromRoute] Guid id,
            EditDogEndpointRequest request,
            [FromServices] ICurrentUser currentUser,
            [FromServices] IQueryDispatcher queryDispatcher,
            [FromServices] ICommandDispatcher commandDispatcher,
            HttpContext httpContext) =>
        {
            var ownerId = currentUser.Id!.Value;

            // Ensure the dog exists and belongs to the current user
            var query = new GetDogQuery(id, ownerId);
            var response = await queryDispatcher.DispatchAsync(query, CancellationToken.None);
            if (response is null)
            {
                return Results.NotFound();
            }

            // Apply updates
            var command = new EditDogCommand(
                id,
                ownerId,
                request.Name,
                request.Breed,
                DateOnly.Parse(request.DateOfBirth),
                request.Sex);

            await commandDispatcher.DispatchAsync(command, CancellationToken.None);

            return Results.NoContent();
        });
    }
}
