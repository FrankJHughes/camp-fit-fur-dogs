using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Microsoft.AspNetCore.Mvc;
using CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;
using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Api.Endpoints.Dogs;

public class ListDogsByCurrentUserEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/dogs", async (
            [FromServices] ICurrentUser currentUser,
            IQueryDispatcher dispatcher) =>
        {
            var query = new ListDogsByOwnerQuery(currentUser.Id!.Value);
            var queryResponse = await dispatcher.DispatchAsync(query, CancellationToken.None);

            var endpointResponse = new ListDogsByCurrentUserEndpointResponse(
                Dogs: [.. queryResponse.Dogs.Select(dog =>
                    new GetDogSummaryEndpointResponse(
                        Id: dog.Id,
                        Name: dog.Name,
                        Breed: dog.Breed
                    ))]);

            return Results.Ok(endpointResponse);
        });
    }
}
