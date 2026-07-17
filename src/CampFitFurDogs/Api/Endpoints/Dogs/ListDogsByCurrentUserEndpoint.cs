using CampFitFurDogs.Application.Abstractions.Dog.ListDogsByOwner;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Endpoints.Dogs;

public class ListDogsByCurrentUserEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dogs", async (
            [FromServices] ICurrentUser currentUser,
            IQueryDispatcher dispatcher) =>
        {
            var query = new ListDogsByOwnerQuery(currentUser.Id!.Value);
            var result = await dispatcher.DispatchAsync(query, CancellationToken.None);
            return Results.Ok(result);
        });
    }
}
