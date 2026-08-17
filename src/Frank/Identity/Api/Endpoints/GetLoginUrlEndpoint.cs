#nullable enable

using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Application.Abstractions.Sessions.Oidc;
using Frank.Core.Domain.Exceptions;
using Frank.Identity.Api.Abstractions.Endpoints;
using Frank.Identity.Api.Settings;
using Frank.Identity.Infrastructure.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Frank.Identity.Api.Endpoints;

/// <summary>
/// Defines the endpoint that generates the next URL required to begin the external
/// OpenID Connect (OIDC) login flow.
/// <para>
/// This endpoint constructs the authorization URL for the identity provider,
/// including the encoded <c>state</c> parameter and the correct callback URL.
/// It validates OIDC and frontend configuration, ensures safe redirect behavior,
/// and returns a minimal DTO containing only the next URL the client must visit.
/// </para>
/// </summary>
/// <remarks>
/// This endpoint follows the Identity purity rules described in US‑110, US‑111,
/// US‑133, and US‑148.
/// </remarks>
public sealed class GetLoginUrlEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the login‑URL endpoint to <c>/identity/login-url</c>.
    /// <para>
    /// The <c>/api</c> prefix is applied automatically by the API route group
    /// created in <c>MapRegisteredApiEndpoints("/api")</c>.
    /// </para>
    /// </summary>
    /// <param name="api">The API route group created by Frank.Core.</param>
    public void Map(RouteGroupBuilder api)
    {
        api.MapGet("/identity/login-url", HandleAsync)
           .AllowAnonymous();
    }

    /// <summary>
    /// Handles the OIDC login‑URL request.
    /// </summary>
    private async Task<IResult> HandleAsync(
        HttpContext http,
        [FromServices] IOptionsMonitor<OidcSettings> oidcOptionsMonitor,
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

            // NOTE: group-relative route → "/identity/callback"
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
