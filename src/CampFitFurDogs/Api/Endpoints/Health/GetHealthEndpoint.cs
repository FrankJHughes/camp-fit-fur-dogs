using Frank.Core.Application.Abstractions.Endpoints;

namespace CampFitFurDogs.Api.Endpoints.Health;

/// <summary>
/// Exposes a simple health‑check endpoint used to verify that the API is running
/// and able to respond to HTTP requests.
/// <para>
/// This endpoint is intentionally lightweight and anonymous, making it suitable
/// for uptime monitoring, load balancer probes, and external service checks.
/// </para>
/// </summary>
public class GetHealthEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the <c>/health</c> route to a basic health‑check response.
    /// <para>
    /// The <c>/api</c> prefix is applied automatically by the API group created
    /// in <c>MapRegisteredApiEndpoints("/api")</c>.
    /// </para>
    /// </summary>
    /// <param name="api">The API route group created by Frank.Core.</param>
    public void Map(RouteGroupBuilder api)
    {
        api.MapGet("/health", () => Results.Ok(new { Status = "Up" }))
            .WithName("GetHealth")
            .AllowAnonymous();
    }
}
