using System.Security.Claims;
using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using Frank.Abstractions.Authentication.Callback;
using CampFitFurDogs.Domain.Errors;
using Frank.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Frank.Authentication.Callback.Oidc;
using Frank.Abstractions.ImmutableContext;

namespace CampFitFurDogs.Api.Verticals.Authentication.Callback;

public class AuthCallbackEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/callback", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext http,
        IHostEnvironment env,
        [FromServices] IImmutableContextBuilder<
            FrankAuthCallbackRequest,
            OidcAuthCallbackContext,
            FrankAuthCallbackResult> frankEngine,
        [FromServices] IImmutableContextBuilder<
            ApplicationAuthCallbackRequest,
            ApplicationAuthCallbackContext,
            ApplicationAuthCallbackContextBuilderResult> appEngine)
    {
        // 1. Extract authorization code
        var code = http.Request.Query["code"].ToString();
        if (string.IsNullOrWhiteSpace(code))
            throw new BadRequestException("Missing authorization code.");

        // 2. Run Frank pipeline (protocol layer)
        var frankAuthCallbackRequest = new FrankAuthCallbackRequest
        {
            Code = code
        };
        var frankAuthCallbackResult = await frankEngine.BuildAsync(frankAuthCallbackRequest, CancellationToken.None);

        // 3. Run Application pipeline (business layer)
        var appAuthCallbackRequest = new ApplicationAuthCallbackRequest
        {
            External = frankAuthCallbackResult,
            Now = DateTimeOffset.UtcNow
        };
        var appAuthCallbackResult = await appEngine.BuildAsync(appAuthCallbackRequest, CancellationToken.None);

        // 4. Issue authentication cookie
        await IssueAuthenticationCookie(
            http,
            appAuthCallbackResult.CustomerId,
            frankAuthCallbackResult.SubjectId
        );

        // 5. Redirect user
        return Results.Redirect(appAuthCallbackResult.RedirectUrl);
    }

    private static async Task IssueAuthenticationCookie(
        HttpContext http,
        Guid customerId,
        string externalSub)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, customerId.ToString()),
            new("sub", externalSub)
        };

        var identity = new ClaimsIdentity(claims, "cfd.session");
        var principal = new ClaimsPrincipal(identity);

        await http.SignInAsync("cfd.session", principal);
    }
}
