using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;

namespace CampFitFurDogs.Api.Dogs;

public static class GetDogProfileEndpoint
{
    public static void MapGetDogProfile(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", async (
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
