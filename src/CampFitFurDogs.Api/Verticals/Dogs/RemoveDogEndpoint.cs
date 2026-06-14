using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using Frank.Abstractions;
using Frank.Api;

namespace CampFitFurDogs.Api.Verticals.Dogs;

public class RemoveDogEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/dogs/{id:guid}", async (
            Guid id,
            ICurrentUser currentUser,
            ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher) =>
        {
            var userId = currentUser.Id;

            var query = new GetDogProfileQuery(DogId: id, CustomerId: userId);
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
