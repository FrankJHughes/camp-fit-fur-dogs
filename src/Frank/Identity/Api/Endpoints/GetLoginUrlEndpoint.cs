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
/// US‑133, and US‑148:
/// <list type="bullet">
/// <item><description>No identity provider tokens are exposed.</description></item>
/// <item><description>No domain logic is embedded in the endpoint.</description></item>
/// <item><description>All sensitive operations occur inside the OIDC and Application pipelines.</description></item>
/// <item><description>The endpoint returns only safe, client‑consumable redirect information.</description></item>
/// </list>
/// </remarks>
public class GetLoginUrlEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the login‑URL endpoint to <c>/api/identity/login-url</c>.
    /// <para>
    /// This endpoint is anonymous because unauthenticated clients must be able to
    /// request the login URL to begin the OIDC flow.
    /// </para>
    /// </summary>
    /// <param name="app">The route builder used to register the endpoint.</param>
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/login-url", HandleAsync)
            .AllowAnonymous();
    }

    /// <summary>
    /// Handles the OIDC login‑URL request.
    /// <para>
    /// The login‑URL flow consists of:
    /// <list type="number">
    /// <item><description>Validate OIDC configuration (authority, client ID, callback).</description></item>
    /// <item><description>Validate frontend configuration (base URL).</description></item>
    /// <item><description>Determine the callback URL (explicit or derived).</description></item>
    /// <item><description>Determine the <c>return_url</c> (query parameter or frontend base URL).</description></item>
    /// <item><description>Encode the OIDC <c>state</c> parameter.</description></item>
    /// <item><description>Construct the full authorization URL for the identity provider.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="http">The current HTTP context.</param>
    /// <param name="oidcOptionsMonitor">Provides the current OIDC configuration.</param>
    /// <param name="frontendOptionsMonitor">Provides the current frontend configuration.</param>
    /// <param name="config">The application configuration.</param>
    /// <returns>A result containing the next URL the client must navigate to.</returns>
    /// <exception cref="BadConfigurationException">
    /// Thrown when required OIDC or frontend configuration is missing or malformed.
    /// </exception>
    /// <exception cref="BadRequestException">
    /// Thrown when the <c>return_url</c> query parameter is present but malformed.
    /// </exception>
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
