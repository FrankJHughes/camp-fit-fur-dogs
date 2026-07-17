using Frank.Core.Application.Abstractions.Authentication.Oidc;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Domain.Exceptions;
using Frank.Identity.Api.Abstractions.Endpoints;
using Frank.Identity.Application.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Frank.Identity.Api.Endpoints;

public class GetLoginUrlEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/login", HandleAsync)
            .AllowAnonymous();
    }

    private async Task<IResult> HandleAsync(
        HttpContext http,
        [FromServices] IOptionsMonitor<OidcCallbackSettings> oidcOptionsMonitor,
        [FromServices] IOptionsMonitor<FrontendSettings> frontendOptionsMonitor,
        IConfiguration config)
    {
        var oidcOptions = oidcOptionsMonitor.CurrentValue;

        var authority = oidcOptions.Authority;
        var clientId = oidcOptions.ClientId;
        if (string.IsNullOrWhiteSpace(authority) ||
            string.IsNullOrWhiteSpace(clientId))
        {
            throw new BadConfigurationException("Authentication configuration is missing or incomplete.");
        }

        var frontendBaseUrl = frontendOptionsMonitor.CurrentValue?.BaseUrl;
        if (string.IsNullOrWhiteSpace(frontendBaseUrl))
        {
            throw new BadConfigurationException("Frontend configuration is missing or incomplete.");
        }

        var callback = oidcOptions.CallbackUrl;
        if (string.IsNullOrWhiteSpace(callback))
        {
            // Build callback from current request domain
            var scheme = http.Request.Scheme;
            var host = http.Request.Host.Value;
            var pathBase = http.Request.PathBase.Value?.TrimEnd('/') ?? "";

            callback = $"{scheme}://{host}{pathBase}/api/identity/callback";
        }

        // Capture return_url (client-specified post-login redirect)
        if (http.Request.Query.TryGetValue("return_url", out var returnUrl))
        {
            if (!Uri.TryCreate(returnUrl, UriKind.RelativeOrAbsolute, out _))
            {
                throw new BadRequestException("malformed return_url query string parameter value");
            }
        }
        else
        {
            if (!Uri.TryCreate(frontendBaseUrl, UriKind.RelativeOrAbsolute, out _))
            {
                throw new BadConfigurationException("malformed Frontend:BaseUrl configuration value");
            }
            returnUrl = frontendBaseUrl;
        }

        // Encode state as JSON
        var decodedState = new Dictionary<string, string>()
        {
            ["return_url"] = returnUrl!
        };
        OidcStateEncoder.TryEncodeValue(decodedState, out var encodedState);

        var scope = "openid profile email";

        var nextUrl =
            $"{authority.TrimEnd('/')}/authorize" +
            $"?response_type=code" +
            $"&client_id={Uri.EscapeDataString(clientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(callback)}" +
            $"&scope={Uri.EscapeDataString(scope)}" +
            $"&state={Uri.EscapeDataString(encodedState!)}";

        return Results.Ok(
            new GetLoginUrlEndpointResponse(nextUrl));
    }
}
