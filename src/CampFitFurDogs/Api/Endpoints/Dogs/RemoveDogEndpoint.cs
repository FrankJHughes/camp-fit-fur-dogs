using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Microsoft.AspNetCore.Mvc;
using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Api.Endpoints.Dogs;

public class RemoveDogEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/dogs/{id:guid}", async (
            Guid id,
            [FromServices] ICurrentUser currentUser,
            ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher) =>
        {
            var userId = currentUser.Id!.Value;

            var query = new GetDogProfileQuery(DogId: id, OwnerId: userId);
            var response = await queryDispatcher.DispatchAsync(query, CancellationToken.None);
            if (response is null)
            {
                return Results.NotFound();
            }

            var command = new RemoveDogCommand(DogId: id, OwnerId: userId);

            await commandDispatcher.DispatchAsync(command, CancellationToken.None);

            return Results.NoContent();
        });
    }
}
