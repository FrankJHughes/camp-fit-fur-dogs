using Microsoft.AspNetCore.Builder;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using SharedKernel.Abstractions;
using SharedKernel.Api;

namespace CampFitFurDogs.Api.Dogs;

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
