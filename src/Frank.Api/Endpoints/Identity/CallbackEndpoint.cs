using Frank.Authentication.Callback.Oidc;
using Frank.Abstractions.ImmutableContext;
using Frank.Abstractions.Authentication.Oidc;
using Frank.Abstractions.Endpoints;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Frank.Abstractions.Identity.Callback;
using Frank.Application.Abstractions.Identity.Callback;
using Frank.Domain.Errors;
using Microsoft.AspNetCore.Builder;

namespace Frank.Api.Endpoints.Identity;

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
            FrankAuthCallbackRequest,
            OidcAuthCallbackContext,
            FrankAuthCallbackResult> frankEngine,
        IImmutableContextBuilder<
            ApplicationAuthCallbackRequest,
            ApplicationAuthCallbackContext,
            ApplicationAuthCallbackContextBuilderResult> appEngine)
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
        var frankAuthCallbackRequest =
            new FrankAuthCallbackRequest
            {
                Code = code!
            };

        var frankAuthCallbackResult =
            await frankEngine.BuildAsync(frankAuthCallbackRequest, CancellationToken.None);

        // 3. Run Application pipeline
        var appAuthCallbackRequest = new ApplicationAuthCallbackRequest
        {
            External = frankAuthCallbackResult,
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
