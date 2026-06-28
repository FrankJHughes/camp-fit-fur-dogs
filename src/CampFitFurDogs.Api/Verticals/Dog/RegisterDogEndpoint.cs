using System.Diagnostics;
using CampFitFurDogs.Application.Abstractions.Dog.RegisterDog;
using Frank.Abstractions;
using Frank.Abstractions.Command;
using Frank.Abstractions.Identity;

namespace CampFitFurDogs.Api.Verticals.Dog;

public class RegisterDogEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/dogs", async (
            RegisterDogRequest request,
            ICurrentUser currentUser,
            ICommandDispatcher dispatcher,
            HttpContext httpContext) =>
        {
            Debug.WriteLine("IsAuthenticated = {Auth}", httpContext.User.Identity?.IsAuthenticated.ToString());
            Debug.WriteLine("Name = {Name}", httpContext.User.Identity?.Name);

            Console.WriteLine($"Received RegisterDogRequest from user {currentUser.Id}");
            var command = new RegisterDogCommand(
                currentUser.Id!.Value,
                request.Name,
                request.Breed,
                DateOnly.Parse(request.DateOfBirth),
                request.Sex);

            var result = await dispatcher.DispatchAsync(command, CancellationToken.None);

            return Results.Created($"/api/dogs/{result}", new { DogId = result });
        })
        .DisableCookieRedirect();
    }
}
