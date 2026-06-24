using System.Diagnostics;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using Frank.Abstractions;

namespace CampFitFurDogs.Api.Verticals.Dogs;

public class RegisterDogEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/dogs", async (
            RegisterDogRequest request,
            ICurrentUser currentUserService,
            ICommandDispatcher dispatcher,
            HttpContext httpContext) =>
        {
            Debug.WriteLine("IsAuthenticated = {Auth}", httpContext.User.Identity?.IsAuthenticated.ToString());
            Debug.WriteLine("Name = {Name}", httpContext.User.Identity?.Name);

            Console.WriteLine($"Received RegisterDogRequest from user {currentUserService.Id}");
            var command = new RegisterDogCommand(
                currentUserService.Id,
                request.Name,
                request.Breed,
                DateOnly.Parse(request.DateOfBirth),
                request.Sex);

            var result = await dispatcher.DispatchAsync(command, CancellationToken.None);

            return Results.Created($"/api/dogs/{result}", new { DogId = result });
        });
    }
}
