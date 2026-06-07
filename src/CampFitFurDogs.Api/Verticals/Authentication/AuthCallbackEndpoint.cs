using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Errors;
using Frank.Api;
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

        IssueSessionCookie(http, result.CustomerId.Value, env);

        return Results.Redirect(result.RedirectUrl);
    }

    private static void IssueSessionCookie(
        HttpContext http,
        Guid customerId,
        IHostEnvironment env)
    {
        http.Response.Cookies.Append(
            "cfd.session",
            customerId.ToString(),
            new CookieOptions
            {
                HttpOnly = true,
                Secure = env.IsProduction(),
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
    }
}
