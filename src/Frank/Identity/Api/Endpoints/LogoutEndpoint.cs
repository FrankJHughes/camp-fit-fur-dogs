using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Domain.Exceptions;
using Frank.Identity.Api.Abstractions.Endpoints;
using Frank.Identity.Application.Abstractions.Sessions.RevokeSession;
using Frank.Identity.Application.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Frank.Identity.Application.Abstractions.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace Frank.Identity.Api.Endpoints;

public class LogoutEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/logout", HandleAsync)
            .AllowAnonymous();
    }

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
            string tokenHash;
            try
            {
                tokenHash = tokenService.Hash(plaintextToken).Value;
                var command = new RevokeSessionCommand(tokenHash);
                await commandDispatcher.DispatchAsync(command, CancellationToken.None);
            }
            catch
            {
            }
        }
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
