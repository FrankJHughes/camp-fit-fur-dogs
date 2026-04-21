using Microsoft.AspNetCore.Builder;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using SharedKernel.Abstractions;
using SharedKernel.Api;

namespace CampFitFurDogs.Api.Dogs;

public class GetDogProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dogs/{id}", async (
            Guid id,
            ICurrentUserService currentUser,
            IQueryDispatcher dispatcher) =>
        {
            var query = new GetDogProfileQuery(id, currentUser.CurrentUserId);
            var result = await dispatcher.DispatchAsync(query, CancellationToken.None);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        });
    }
}
