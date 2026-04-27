using Microsoft.AspNetCore.Builder;
using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using SharedKernel.Abstractions;
using SharedKernel.Api;

namespace CampFitFurDogs.Api.Dogs;

public class ListDogsByCurrentUserEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dogs", async (
            ICurrentUserService currentUser,
            IQueryDispatcher dispatcher) =>
        {
            var query = new ListDogsByOwnerQuery(currentUser.CurrentUserId);
            var result = await dispatcher.DispatchAsync(query, CancellationToken.None);
            return Results.Ok(result);
        });
    }
}
