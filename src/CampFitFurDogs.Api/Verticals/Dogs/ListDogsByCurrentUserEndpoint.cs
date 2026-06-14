using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using Frank.Abstractions;
using Frank.Api;

namespace CampFitFurDogs.Api.Verticals.Dogs;

public class ListDogsByCurrentUserEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dogs", async (
            ICurrentUser currentUser,
            IQueryDispatcher dispatcher) =>
        {
            var query = new ListDogsByOwnerQuery(currentUser.Id);
            var result = await dispatcher.DispatchAsync(query, CancellationToken.None);
            return Results.Ok(result);
        });
    }
}
