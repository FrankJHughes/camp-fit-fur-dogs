#nullable enable

using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Application.Abstractions.Sessions.Oidc;
using Frank.Core.Domain.Exceptions;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using CallbackSaveContextBuilderRequest = Frank.Identity.Application.Abstractions.Callback.Save.CallbackSaveContextBuilderRequest;

namespace Frank.Identity.Api.Endpoints;

/// <summary>
/// Defines the OIDC callback endpoint used by the Identity API.
/// <para>
/// This endpoint completes the external OpenID Connect (OIDC) authentication flow
/// by processing the <c>state</c> and <c>code</c> query parameters returned by the
/// identity provider.
/// It validates the callback payload, runs the OIDC pipeline, runs the application
/// pipeline, issues the session cookie, and finally redirects the user back to the
/// URL encoded in the OIDC state.
/// </para>
/// </summary>
/// <remarks>
/// This endpoint follows the Identity purity rules described in US‑110, US‑111,
/// and US‑133:
/// <list type="bullet">
/// <item><description>No identity provider tokens are exposed to the client.</description></item>
/// <item><description>No domain logic is embedded in the endpoint.</description></item>
/// <item><description>All sensitive operations occur inside the OIDC and Application pipelines.</description></item>
/// <item><description>The final session cookie is issued only after successful pipeline execution.</description></item>
/// </list>
/// </remarks>
public class CallbackEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the OIDC callback endpoint to <c>/api/identity/callback</c>.
    /// <para>
    /// This endpoint is marked <c>AllowAnonymous</c> because the identity provider
    /// redirects unauthenticated users back to this URL during the login flow.
    /// </para>
    /// </summary>
    /// <param name="app">The route builder used to register the endpoint.</param>
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/callback", HandleAsync)
            .AllowAnonymous();
    }

    /// <summary>
    /// Handles the OIDC callback request.
    /// <para>
    /// The callback flow consists of:
    /// <list type="number">
    /// <item><description>Extract and decode the OIDC <c>state</c> parameter.</description></item>
    /// <item><description>Extract the authorization <c>code</c> if present.</description></item>
    /// <item><description>Run the OIDC pipeline to exchange the code for external identity data.</description></item>
    /// <item><description>Run the application pipeline to convert external identity into a session cookie.</description></item>
    /// <item><description>Issue the session cookie and redirect the user to the original <c>return_url</c>.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="http">The current HTTP context.</param>
    /// <param name="env">The hosting environment.</param>
    /// <param name="oidcContextBuilder">Builds the OIDC callback context.</param>
    /// <param name="saveContextBuilder">Builds the application callback context and session cookie.</param>
    /// <returns>A redirect result pointing to the original <c>return_url</c>.</returns>
    /// <exception cref="BadRequestException">
    /// Thrown when required OIDC callback parameters are missing or malformed.
    /// </exception>
    private static async Task<IResult> HandleAsync(
        HttpContext http,
        IHostEnvironment env,
        [FromServices] ICallbackOidcContextBuilder oidcContextBuilder,
        [FromServices] ICallbackSaveContextBuilder saveContextBuilder)
    {
        // 1a. Extract and decode state
        var query = http.Request.Query;
        if (!query.TryGetValue("state", out var encodedState))
        {
            throw new BadRequestException("missing state query string parameter");
        }

        if (!OidcStateEncoder.TryDecodeValue<Dictionary<string, string>>(encodedState!, out var decodedState))
        {
            throw new BadRequestException("malformed state query string parameter value");
        }

        if (!decodedState!.TryGetValue("return_url", out var returnUrl))
        {
            throw new BadRequestException("missing return_url state parameter value");
        }

        if (!Uri.TryCreate(returnUrl, UriKind.RelativeOrAbsolute, out var uri))
        {
            throw new BadRequestException("malformed return_url state parameter value");
        }

        // 1b. Extract authorization code
        if (!http.Request.Query.TryGetValue("code", out var code))
        {
            // 5. Redirect user
            return Results.Redirect(returnUrl);
        }

        // 2. Run Frank pipeline
        var oidcCallbackRequest =
            new CallbackOidcContextBuilderRequest
            {
                Code = code!
            };

        var oidcCallbackResult =
            await oidcContextBuilder.BuildAsync(oidcCallbackRequest, CancellationToken.None);

        // 3. Run Application pipeline
        var appAuthCallbackRequest = new CallbackSaveContextBuilderRequest
        {
            External = oidcCallbackResult,
            Now = DateTimeOffset.UtcNow
        };
        var appAuthCallbackResult = await saveContextBuilder.BuildAsync(appAuthCallbackRequest, CancellationToken.None);

        if (string.IsNullOrWhiteSpace(appAuthCallbackResult.CookieValue))
        {
            return Results.Redirect(returnUrl);
        }

        // 4. Issue the REAL CFFD session cookie
        http.Response.Cookies.Append(
            "session",
            appAuthCallbackResult.CookieValue,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
                // No Expires — the cookie value already encodes expiration
            });

        // 5. Redirect user
        return Results.Redirect(returnUrl);
    }
}
