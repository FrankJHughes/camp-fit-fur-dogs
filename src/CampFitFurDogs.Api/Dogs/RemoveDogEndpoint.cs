using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using SharedKernel.Abstractions;
using SharedKernel.Api;

namespace CampFitFurDogs.Api.Dogs;

public class RemoveDogEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/dogs/{id:guid}", async (
            Guid id,
            ICurrentUserService currentUserService,
            ICommandDispatcher dispatcher) =>
        {
            var command = new RemoveDogCommand(
                DogId: id,
                OwnerId: currentUserService.CurrentUserId);

            await dispatcher.DispatchAsync(command, CancellationToken.None);

            return Results.NoContent();
        });
    }
}
