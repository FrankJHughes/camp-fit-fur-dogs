using Frank.Abstractions;

namespace CampFitFurDogs.Api.Verticals.Authentication;

public class AuthSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/session", (HttpContext http) =>
        {
            var isAuthenticated = http.User?.Identity?.IsAuthenticated ?? false;
            return Results.Json(new { isAuthenticated });
        });
    }
}
