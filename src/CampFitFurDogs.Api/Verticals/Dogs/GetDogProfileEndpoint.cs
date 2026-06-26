using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using Frank.Abstractions;
using Frank.Abstractions.Identity;
using Frank.Abstractions.Query;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Verticals.Dogs;

public class GetDogProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dogs/{id}", async (
            Guid id,
            [FromServices] ICurrentUser currentUser,
            IQueryDispatcher dispatcher) =>
        {
            var query = new GetDogProfileQuery(id, currentUser.Id!.Value);
            var result = await dispatcher.DispatchAsync(query, CancellationToken.None);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        });
    }
}
