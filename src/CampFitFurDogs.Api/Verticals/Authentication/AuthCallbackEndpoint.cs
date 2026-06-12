using System.Security.Claims;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Errors;
using Frank.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Verticals.Authentication;

public class AuthCallbackEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/callback", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext http,
        IHostEnvironment env,
        [FromServices] IAuthCallbackService service)
    {
        var code = http.Request.Query["code"].ToString();
        if (string.IsNullOrWhiteSpace(code))
            throw new BadRequestException("Missing authorization code.");

        var result = await service.HandleAsync(code, CancellationToken.None);

        await IssueAuthenticationCookie(http, result.CustomerId.Value, env.EnvironmentName);

        return Results.Redirect(result.RedirectUrl);
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
