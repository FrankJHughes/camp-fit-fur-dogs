using Frank.Abstractions.Endpoints;

namespace CampFitFurDogs.Api.Endpoints.Health;

public class GetHealthEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/api/health", () => Results.Ok(new { Status = "Up" }))
            .WithName("GetHealth")
            .AllowAnonymous();
    }
}
