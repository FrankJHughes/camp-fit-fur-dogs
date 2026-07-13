using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Core.Application.Abstractions.Authentication.Oidc;
using Frank.Core.Application.Abstractions.Endpoints;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Frank.Core.Domain.Errors;

using Microsoft.AspNetCore.Builder;
using Frank.Identity.Application.Abstractions.Callback.Save;
using SaveCallbackContextBuilderRequest = Frank.Identity.Application.Abstractions.Callback.Save.SaveCallbackContextBuilderRequest;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Api.Endpoints;

public class CallbackEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/callback", HandleAsync)
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        HttpContext http,
        IHostEnvironment env,
        IImmutableContextBuilder<
            OidcCallbackContextBuilderRequest,
            OidcCallbackContext,
            OidcCallbackContextBuilderResult> frankEngine,
        IImmutableContextBuilder<
            SaveCallbackContextBuilderRequest,
            SaveCallbackContext,
            SaveCallbackContextBuilderResult> appEngine)
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
            new OidcCallbackContextBuilderRequest
            {
                Code = code!
            };

        var oidcCallbackResult =
            await frankEngine.BuildAsync(oidcCallbackRequest, CancellationToken.None);

        // 3. Run Application pipeline
        var appAuthCallbackRequest = new SaveCallbackContextBuilderRequest
        {
            External = oidcCallbackResult,
            Now = DateTimeOffset.UtcNow
        };
        var appAuthCallbackResult = await appEngine.BuildAsync(appAuthCallbackRequest, CancellationToken.None);

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
