using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SharedKernel.Api;
using CampFitFurDogs.Api.Errors;
using CampFitFurDogs.Domain.Errors;

namespace CampFitFurDogs.Api.Authentication;

public class LoginEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/login", async (HttpContext http) =>
        {
            // Auth0 configuration (from appsettings or environment)
            var domain = http.RequestServices
                .GetRequiredService<IConfiguration>()["Authentication:Oidc:Authority"];

            var clientId = http.RequestServices
                .GetRequiredService<IConfiguration>()["Authentication:Oidc:ClientId"];

            var callback = http.RequestServices
                .GetRequiredService<IConfiguration>()["Authentication:Oidc:CallbackUrl"];

            if (string.IsNullOrWhiteSpace(domain) ||
                string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(callback))
            {
                throw new BadConfigurationException(
                    "Auth0 configuration is missing or incomplete.");
            }

            // Generate a secure state parameter
            var state = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var scope = "openid profile email";

            // Build the Auth0 authorization URL
            var redirectUrl =
                $"https://{domain}/authorize" +
                $"?response_type=code" +
                $"&client_id={Uri.EscapeDataString(clientId)}" +
                $"&redirect_uri={Uri.EscapeDataString(callback)}" +
                $"&scope={Uri.EscapeDataString(scope)}" +
                $"&state={Uri.EscapeDataString(state)}";

            return Results.Redirect(redirectUrl);
        });
    }
}
