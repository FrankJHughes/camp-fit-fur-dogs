using System.Security.Claims;
using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using Frank.Abstractions.Authentication.Callback;
using CampFitFurDogs.Domain.Errors;
using Microsoft.AspNetCore.Authentication;
using Frank.Authentication.Callback.Oidc;
using Frank.Abstractions.ImmutableContextBuilder;
using System.Text.Json;
using Frank.Abstractions;

namespace CampFitFurDogs.Api.Vertical.Authentication.Callback;

public class AuthCallbackEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/callback", HandleAsync)
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
        // 1. Extract authorization code
        var code = http.Request.Query["code"].ToString();
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new BadRequestException("Missing authorization code.");
        }

        // 1b. Extract and decode state
        var stateRaw = http.Request.Query["state"].ToString();
        string? requestedRedirectUrl = null;

        if (!string.IsNullOrWhiteSpace(stateRaw))
        {
            try
            {
                var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(stateRaw));
                var stateObj = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (stateObj != null && stateObj.TryGetValue("r", out var r))
                {
                    requestedRedirectUrl = r;
                }
            }
            catch
            {
                // Ignore malformed state
            }
        }

        // 2. Run Frank pipeline
        var frankAuthCallbackRequest = new FrankAuthCallbackRequest
        {
            Code = code
        };
        var frankAuthCallbackResult = await frankEngine.BuildAsync(frankAuthCallbackRequest, CancellationToken.None);

        // 3. Run Application pipeline
        var appAuthCallbackRequest = new ApplicationAuthCallbackRequest
        {
            External = frankAuthCallbackResult,
            Now = DateTimeOffset.UtcNow,
            RequestedRedirectUrl = requestedRedirectUrl
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
