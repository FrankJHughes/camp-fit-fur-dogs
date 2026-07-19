using CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Microsoft.AspNetCore.Mvc;
using CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;


namespace CampFitFurDogs.Api.Endpoints.Dogs;

public class EditDogProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/dogs/{id}", async (
            [FromRoute] Guid id,
            EditDogProfileEndpointRequest request,
            ICurrentUser currentUser,
            IQueryDispatcher queryDispatcher,
            ICommandDispatcher commandDispatcher,
            HttpContext httpContext) =>
        {
            var ownerId = currentUser.Id!.Value;

            var query = new GetDogProfileQuery(id, ownerId);
            var response = await queryDispatcher.DispatchAsync(query, CancellationToken.None);
            if (response is null)
            {
                return Results.NotFound();
            }

            var command = new EditDogProfileCommand(
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
