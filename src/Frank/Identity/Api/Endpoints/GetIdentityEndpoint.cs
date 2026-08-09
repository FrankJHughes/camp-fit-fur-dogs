#nullable enable

using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Api.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Frank.Identity.Api.Endpoints;

/// <summary>
/// Defines the endpoint that returns the authenticated user's resolved identity
/// information.
/// <para>
/// This endpoint exposes a minimal, safe identity DTO containing only the user's
/// display name.
/// It intentionally avoids returning claims, tokens, provider metadata, or any
/// sensitive identity information, in alignment with the Identity purity rules
/// described in US‑110 and US‑111.
/// </para>
/// </summary>
/// <remarks>
/// This endpoint requires authentication and represents the simplest identity
/// surface in the Identity API:
/// <list type="bullet">
/// <item><description>Returns only safe, client‑consumable identity data.</description></item>
/// <item><description>Delegates identity resolution to <see cref="ICurrentUser"/>.</description></item>
/// <item><description>Contains no domain logic or provider‑specific behavior.</description></item>
/// </list>
/// </remarks>
public class GetIdentityEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the identity endpoint to <c>/api/identity</c>.
    /// <para>
    /// This endpoint requires authorization and returns the authenticated user's
    /// resolved identity information using <see cref="ICurrentUser"/>.
    /// </para>
    /// </summary>
    /// <param name="app">The route builder used to register the endpoint.</param>
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
