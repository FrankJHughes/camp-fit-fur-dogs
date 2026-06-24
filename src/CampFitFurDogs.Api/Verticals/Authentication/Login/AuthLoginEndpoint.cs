using System.Text.Json;
using CampFitFurDogs.Application.Settings;
using CampFitFurDogs.Domain.Errors;
using Frank.Abstractions;
using Frank.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.Api.Verticals.Authentication.Login;

public class AuthLoginEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/login", (
            HttpContext http,
            [FromServices] IOptionsMonitor<AuthCallbackOidcSettings> oidcOptionsMonitor,
            [FromServices] IOptionsMonitor<FrontendSettings> frontendOptionsMonitor,
            IConfiguration config) =>
        {
            var oidcOptions = oidcOptionsMonitor.CurrentValue;

            var authority = oidcOptions.Authority;
            var clientId = oidcOptions.ClientId;
            var callback = oidcOptions.CallbackUrl;

            if (string.IsNullOrWhiteSpace(authority) ||
                string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(callback))
            {
                throw new BadConfigurationException("Authentication configuration is missing or incomplete.");
            }

            var frontendBaseUrl = frontendOptionsMonitor.CurrentValue?.BaseUrl;

            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                throw new BadConfigurationException("Frontend configuration is missing or incomplete.");
            }

            // Capture returnUrl (PR preview URL)
            var returnUrl = http.Request.Query["returnUrl"].ToString();
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = frontendBaseUrl;
            }

            // Encode state as JSON
            var stateObj = new { r = returnUrl };
            var stateJson = JsonSerializer.Serialize(stateObj);
            var state = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(stateJson));

            var scope = "openid profile email";

            var redirectUrl =
                $"{authority.TrimEnd('/')}/authorize" +
                $"?response_type=code" +
                $"&client_id={Uri.EscapeDataString(clientId)}" +
                $"&redirect_uri={Uri.EscapeDataString(callback)}" +
                $"&scope={Uri.EscapeDataString(scope)}" +
                $"&state={Uri.EscapeDataString(state)}";

            return Results.Redirect(redirectUrl);
        });
    }
}
