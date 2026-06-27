using CampFitFurDogs.Application.Abstractions.Dog.GetDogProfile;
using CampFitFurDogs.Application.Abstractions.Dog.RemoveDog;
using Frank.Abstractions;
using Frank.Abstractions.Command;
using Frank.Abstractions.Identity;
using Frank.Abstractions.Query;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Vertical.Dog;

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
