using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using Frank.Abstractions;
using Frank.Api;

namespace CampFitFurDogs.Api.Verticals.Dogs;

public class RegisterDogEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/dogs", async (
            RegisterDogRequest request,
            ICurrentUserService currentUserService,
            ICommandDispatcher dispatcher) =>
        {
            Console.WriteLine($"Received RegisterDogRequest from user {currentUserService.CurrentUserId}");
            var command = new RegisterDogCommand(
                currentUserService.CurrentUserId,
                request.Name,
                request.Breed,
                DateOnly.Parse(request.DateOfBirth),
                request.Sex);

            var result = await dispatcher.DispatchAsync(command, CancellationToken.None);

            return Results.Created($"/api/dogs/{result}", new { DogId = result });
        });
    }
}
