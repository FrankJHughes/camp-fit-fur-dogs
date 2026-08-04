using CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.EditDog;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Mvc;


namespace CampFitFurDogs.Api.Endpoints.Dogs;

public class EditDogEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/dogs/{id}", async (
            [FromRoute] Guid id,
            EditDogEndpointRequest request,
            [FromServices] ICurrentUser currentUser,
            [FromServices] IQueryDispatcher queryDispatcher,
            [FromServices] ICommandDispatcher commandDispatcher,
            HttpContext httpContext) =>
        {
            var ownerId = currentUser.Id!.Value;

            var query = new GetDogQuery(id, ownerId);
            var response = await queryDispatcher.DispatchAsync(query, CancellationToken.None);
            if (response is null)
            {
                return Results.NotFound();
            }

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
