using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Endpoints.Dogs;

public class GetDogEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dogs/{id}", async (
            Guid id,
            [FromServices] ICurrentUser currentUser,
            [FromServices] IQueryDispatcher dispatcher) =>
        {
            var query = new GetDogQuery(id, currentUser.Id!.Value);
            var queryResponse = await dispatcher.DispatchAsync(query, CancellationToken.None);
            if (queryResponse is null)
            {
                return Results.NotFound();
            }

            var endpointResponse = new
            {
                Id = queryResponse.Id,
                OwnerId = queryResponse.OwnerId,
                Name = queryResponse.Name,
                Breed = queryResponse.Breed,
                Sex = queryResponse.Sex,
                DateOfBirth = queryResponse.DateOfBirth
            };

            return Results.Ok(endpointResponse);
        });
    }
}
