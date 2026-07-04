using Frank.Abstractions;
using Frank.Abstractions.Identity;

namespace CampFitFurDogs.Api.Verticals.Identity;

public class GetIdentityEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity", (ICurrentUser currentUser) =>
        {
            var response = new GetIdentityResponse
            {
                IsAuthenticated = currentUser.IsAuthenticated,
                User = currentUser.IsAuthenticated
                    ? new IdentityUserDto
                    {
                        Id = currentUser.Id,
                        Name = currentUser.Name
                    }
                    : null
            };

            return Results.Json(response);
        });
    }
}
