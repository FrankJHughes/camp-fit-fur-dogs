using System.Diagnostics;
using CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Endpoints.Dogs;

/// <summary>
/// Handles HTTP POST requests for registering a new dog under the currently
/// authenticated user.
/// <para>
/// This endpoint validates the authenticated user, constructs a registration
/// command, dispatches it through the application command pipeline, and returns
/// a <c>201 Created</c> response containing the newly assigned dog identifier.
/// </para>
/// </summary>
public class RegisterDogEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the <c>/dogs</c> route to the register‑dog operation.
    /// <para>
    /// The <c>/api</c> prefix is applied automatically by the API group created
    /// in <c>MapRegisteredApiEndpoints("/api")</c>.
    /// </para>
    /// </summary>
    /// <param name="api">The API route group created by Frank.Core.</param>
    public void Map(RouteGroupBuilder api)
    {
        api.MapPost("/dogs", async (
            RegisterDogEndpointRequest request,
            [FromServices] ICurrentUser currentUser,
            [FromServices] ICommandDispatcher dispatcher,
            HttpContext httpContext) =>
        {
            Debug.WriteLine("IsAuthenticated = {Auth}", httpContext.User.Identity?.IsAuthenticated.ToString());
            Debug.WriteLine("Name = {Name}", httpContext.User.Identity?.Name);

            Console.WriteLine($"Received RegisterDogRequest from user {currentUser.Id}");

            var command = new RegisterDogCommand(
                currentUser.Id!.Value,
                request.Name,
                request.Breed,
                DateOnly.Parse(request.DateOfBirth),
                request.Sex);

            var commandResponse = await dispatcher.DispatchAsync(command, CancellationToken.None);

            var endpointResponse = new RegisterDogEndpointResponse(commandResponse);

            // IMPORTANT:
            // The location must be relative to the group.
            // The group will prefix it with /api automatically.
            return Results.Created($"/dogs/{commandResponse}", endpointResponse);
        })
        .DisableCookieRedirect();
    }
}
