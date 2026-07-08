using Frank.Abstractions;
using Frank.Abstractions.Identity;

namespace CampFitFurDogs.Api.Verticals.Identity;

public class GetIdentityEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity", (ICurrentUser currentUser) =>
        {
            var dto = new GetIdentityResponse
            {
                Name = currentUser.Name!
            };

            return Results.Ok(dto);
        })
        .RequireAuthorization(); // Require authenticated user for this endpoint
    }
}
