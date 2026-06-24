using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using Frank.Abstractions;

namespace CampFitFurDogs.Api.Verticals.Dogs;

public class GetDogProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dogs/{id}", async (
            Guid id,
            ICurrentUser currentUser,
            IQueryDispatcher dispatcher) =>
        {
            var query = new GetDogProfileQuery(id, currentUser.Id);
            var result = await dispatcher.DispatchAsync(query, CancellationToken.None);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        });
    }
}
