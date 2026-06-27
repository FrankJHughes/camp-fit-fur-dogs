using Frank.Abstractions;

namespace CampFitFurDogs.Api.Vertical.Health;

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
