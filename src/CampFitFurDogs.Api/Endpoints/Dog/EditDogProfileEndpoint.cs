using CampFitFurDogs.Application.Abstractions.Dog.EditDogProfile;
using CampFitFurDogs.Application.Abstractions.Dog.GetDogProfile;
using Frank.Abstractions.Command;
using Frank.Abstractions.Endpoints;
using Frank.Abstractions.Identity;
using Frank.Abstractions.Query;
using Microsoft.AspNetCore.Mvc;


namespace CampFitFurDogs.Api.Endpoints.Dog;

public class EditDogProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/dogs/{id}", async (
            [FromRoute] Guid id,
            EditDogProfileRequest request,
            ICurrentUser currentUser,
            IQueryDispatcher queryDispatcher,
            ICommandDispatcher commandDispatcher,
            HttpContext httpContext) =>
        {
            var ownerId = currentUser.Id!.Value;

            var query = new GetDogProfileQuery(id, ownerId);
            var response = await queryDispatcher.DispatchAsync(query, CancellationToken.None);
            if (response is null)
            {
                return Results.NotFound();
            }

            var command = new EditDogProfileCommand(
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
