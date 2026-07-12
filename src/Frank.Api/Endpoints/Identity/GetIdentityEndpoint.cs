using Frank.Abstractions.Endpoints;
using Frank.Abstractions.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.Api.Endpoints.Identity;

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
