#nullable enable

using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Domain.Exceptions;
using Frank.Identity.Api.Abstractions.Endpoints;
using Frank.Identity.Api.Settings;
using Frank.Identity.Application.Abstractions.Sessions;
using Frank.Identity.Application.Abstractions.Sessions.RevokeSession;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Frank.Identity.Api.Endpoints;

/// <summary>
/// Defines the endpoint responsible for logging out a user from the Identity API.
/// <para>
/// This endpoint revokes the user's session (if present), deletes the session cookie,
/// and returns a redirect URL indicating where the client should navigate after logout.
/// </para>
/// </summary>
/// <remarks>
/// This endpoint follows the Identity purity rules described in US‑110, US‑111, and US‑133.
/// </remarks>
public sealed class LogoutEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the logout endpoint to <c>/identity/logout</c>.
    /// <para>
    /// The <c>/api</c> prefix is applied automatically by the API route group
    /// created in <c>MapRegisteredApiEndpoints("/api")</c>.
    /// </para>
    /// </summary>
    /// <param name="api">The API route group created by Frank.Core.</param>
    public void Map(RouteGroupBuilder api)
    {
        api.MapGet("/identity/logout", HandleAsync)
           .AllowAnonymous();
    }

    /// <summary>
    /// Handles the logout request.
    /// </summary>
    private async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromServices] ISessionTokenGenerator tokenService,
        [FromServices] ICommandDispatcher commandDispatcher,
        IOptionsMonitor<FrontendSettings> frontendOptionsMonitor)
    {
        // 1. Read plaintext token from cookie
        var plaintextToken = httpContext.Request.Cookies["session"];

        // 2. Hash the plaintext token and revoke the session
        if (!string.IsNullOrEmpty(plaintextToken))
        {
            try
            {
                var tokenHash = tokenService.Hash(plaintextToken).Value;
                var command = new RevokeSessionCommand(tokenHash);
                await commandDispatcher.DispatchAsync(command, CancellationToken.None);
            }
            catch
            {
                // Swallow exceptions — logout must continue even if revocation fails
            }
        }

        // 3. Delete the session cookie regardless of revocation outcome
        httpContext.Response.Cookies.Delete("session");

        // 4. Determine post‑logout redirect URL
        var returnUrl = httpContext.Request.Query["return_url"].ToString();

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            var frontendBaseUrl = frontendOptionsMonitor.CurrentValue?.BaseUrl;
            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                throw new BadConfigurationException("Frontend configuration is missing or incomplete.");
            }

            returnUrl = frontendBaseUrl;
        }

        // 5. Return safe redirect information
        return Results.Ok(new LogoutEndpointResponse(returnUrl));
    }
}
