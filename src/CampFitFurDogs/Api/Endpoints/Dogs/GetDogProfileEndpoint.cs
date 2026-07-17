using CampFitFurDogs.Application.Abstractions.Dog.GetDogProfile;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Endpoints.Dogs;

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
