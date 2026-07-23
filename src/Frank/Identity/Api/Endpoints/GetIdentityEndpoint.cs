using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Api.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Frank.Identity.Api.Endpoints;

public class GetIdentityEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity", ([FromServices] ICurrentUser currentUser) =>
        {
            var dto = new GetIdentityEndpointResponse
            {
                Name = currentUser.Name!
            };

            return Results.Ok(dto);
        })
        .RequireAuthorization(); // Require authenticated user for this endpoint
    }
}
