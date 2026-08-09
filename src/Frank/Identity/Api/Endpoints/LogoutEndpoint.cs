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
/// This endpoint follows the Identity purity rules described in US‑110, US‑111, and US‑133:
/// <list type="bullet">
/// <item><description>No identity provider tokens are exposed.</description></item>
/// <item><description>No domain logic is embedded in the endpoint.</description></item>
/// <item><description>Session revocation is delegated to the application pipeline.</description></item>
/// <item><description>The endpoint returns only safe, client‑consumable redirect information.</description></item>
/// </list>
/// </remarks>
public class LogoutEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the logout endpoint to <c>/api/identity/logout</c>.
    /// <para>
    /// This endpoint is anonymous because clients must be able to log out even when
    /// their session has expired or is otherwise invalid.
    /// </para>
    /// </summary>
    /// <param name="app">The route builder used to register the endpoint.</param>
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/logout", HandleAsync)
            .AllowAnonymous();
    }

    /// <summary>
    /// Handles the logout request.
    /// <para>
    /// The logout flow consists of:
    /// <list type="number">
    /// <item><description>Read the plaintext session token from the cookie.</description></item>
    /// <item><description>Hash the token and dispatch a <see cref="RevokeSessionCommand"/>.</description></item>
    /// <item><description>Delete the session cookie.</description></item>
    /// <item><description>Determine the post‑logout redirect URL.</description></item>
    /// <item><description>Return a <see cref="LogoutEndpointResponse"/> containing the redirect URL.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="tokenService">Generates and hashes session tokens.</param>
    /// <param name="commandDispatcher">Dispatches commands to revoke sessions.</param>
    /// <param name="frontendOptionsMonitor">Provides the current frontend configuration.</param>
    /// <returns>A result containing the URL the client should navigate to after logout.</returns>
    /// <exception cref="BadConfigurationException">
    /// Thrown when frontend configuration is missing or malformed.
    /// </exception>
    private async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromServices] ISessionTokenGenerator tokenService,
        [FromServices] ICommandDispatcher commandDispatcher,
        IOptionsMonitor<FrontendSettings> frontendOptionsMonitor)
    {

        // 1. Read plaintext token from cookie
        var plaintextToken = httpContext.Request.Cookies["session"];

        // 2. Hash the plaintext token
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

        // Delete the session cookie regardless of revocation outcome
        httpContext.Response.Cookies.Delete("session");

        //
        // Determine where to redirect after logout.
        //
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

        return Results.Ok(
            new LogoutEndpointResponse(returnUrl));
    }
}
