using CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using Frank.Abstractions;
using Frank.Api;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Verticals.Dogs;

public class EditDogProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/dogs/{id}", async (
            [FromRoute] Guid id,
            EditDogProfileRequest request,
            ICurrentUser currentUserService,
            IQueryDispatcher queryDispatcher,
            ICommandDispatcher commandDispatcher,
            HttpContext httpContext) =>
        {
            var customerId = currentUserService.Id;

            var query = new GetDogProfileQuery(id, customerId);
            var response = await queryDispatcher.DispatchAsync(query, CancellationToken.None);
            if (response is null)
            {
                return Results.NotFound();
            }

            var command = new EditDogProfileCommand(
                id,
                customerId,
                request.Name,
                request.Breed,
                DateOnly.Parse(request.DateOfBirth),
                request.Sex);
            await commandDispatcher.DispatchAsync(command, CancellationToken.None);

            return Results.NoContent();
        });
    }
}
