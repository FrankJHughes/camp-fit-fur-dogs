using Microsoft.AspNetCore.Builder;
using CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;
using SharedKernel.Abstractions;
using SharedKernel.Api;

namespace CampFitFurDogs.Api.Dogs;

public class EditDogProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/dogs/{id}", async (
            Guid id,
            EditDogProfileRequest request,
            ICurrentUserService currentUserService,
            ICommandDispatcher dispatcher) =>
        {
            var command = new EditDogProfileCommand(
                id,
                currentUserService.CurrentUserId,
                request.Name,
                request.Breed,
                DateOnly.Parse(request.DateOfBirth),
                request.Sex);

            await dispatcher.DispatchAsync(command, CancellationToken.None);

            return Results.NoContent();
        });
    }
}
