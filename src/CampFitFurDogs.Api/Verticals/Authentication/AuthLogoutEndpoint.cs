using System.Text.Json;
using CampFitFurDogs.Application.Settings;
using CampFitFurDogs.Domain.Errors;
using Frank.Abstractions;
using Frank.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.Api.Verticals.Authentication;

public class AuthLogoutEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/logout", HandleAsync);
    }

    private async Task<IResult> HandleAsync(
        HttpContext http,
        [FromServices] IOptionsMonitor<AuthCallbackOidcSettings> oidcOptionsMonitor,
        [FromServices] IOptionsMonitor<FrontendSettings> frontendOptionsMonitor,
        IConfiguration config)
    {
        // Clear session cookie
        http.Response.Cookies.Delete("cffd.session");

        // Capture return_url (client-specified post-login redirect)
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

        // Redirect to logged-out page
        return Results.Redirect(returnUrl);
    }
}
