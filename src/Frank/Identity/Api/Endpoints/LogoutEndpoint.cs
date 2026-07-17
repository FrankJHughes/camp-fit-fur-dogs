using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Domain.Exceptions;
using Frank.Identity.Application.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Frank.Identity.Api.Endpoints;

public class LogoutEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/logout", HandleAsync)
            .AllowAnonymous();
    }

    private async Task<IResult> HandleAsync(
        HttpContext http,
        [FromServices] IOptionsMonitor<FrontendSettings> frontendOptionsMonitor)
    {
        //
        // IMPORTANT:
        //
        // We no longer use ASP.NET cookie authentication ("cffd.session").
        // The real authentication cookie is now the domain session cookie: "session".
        //
        // So logout simply deletes the domain session cookie.
        //
        http.Response.Cookies.Delete("session");

        //
        // Determine where to redirect after logout.
        //
        var returnUrl = http.Request.Query["return_url"].ToString();

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
            new LogoutResponse(returnUrl));
    }
}
