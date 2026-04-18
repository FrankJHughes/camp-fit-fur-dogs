using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;

namespace CampFitFurDogs.Api.Dogs;

public static class RegisterDogEndpoint
{
    public static void MapRegisterDog(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
            RegisterDogRequest request,
            ICurrentUserService currentUserService,
            ICommandDispatcher dispatcher) =>
        {
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
