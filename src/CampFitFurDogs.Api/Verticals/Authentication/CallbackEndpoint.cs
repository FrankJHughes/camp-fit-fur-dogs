using System.Security.Claims;
using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using Frank.Abstractions.Authentication.Callback;
using CampFitFurDogs.Domain.Errors;
using Microsoft.AspNetCore.Authentication;
using Frank.Authentication.Callback.Oidc;
using Frank.Abstractions.ImmutableContext;
using System.Text.Json;
using Frank.Abstractions;

namespace CampFitFurDogs.Api.Verticals.Authentication;

public class CallbackEndpoint : IEndpoint
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

                if (stateObj != null && stateObj.TryGetValue("return_url", out var r))
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
            frankAuthCallbackResult.SubjectId,
            appAuthCallbackResult.CustomerId,
            $"{frankAuthCallbackResult.GivenName} {frankAuthCallbackResult.FamilyName}"
        );

        // 5. Redirect user
        return Results.Redirect(appAuthCallbackResult.RedirectUrl);
    }

    private static async Task IssueAuthenticationCookie(
        HttpContext http,
        string externalSub,
        Guid customerId,
        string? displayName)
    {
        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, customerId.ToString()),
        new("sub", externalSub)
    };

        if (!string.IsNullOrWhiteSpace(displayName))
        {
            claims.Add(new(ClaimTypes.Name, displayName));
        }

        var identity = new ClaimsIdentity(claims, "cfd.session");
        var principal = new ClaimsPrincipal(identity);

        await http.SignInAsync("cfd.session", principal);
    }
}
