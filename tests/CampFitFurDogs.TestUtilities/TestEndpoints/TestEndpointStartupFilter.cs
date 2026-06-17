using CampFitFurDogs.Application.Abstractions.Identity;
using CampFitFurDogs.Infrastructure.Data;
using Frank.Abstractions.Authentication.Callback;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace CampFitFurDogs.TestUtilities.TestEndpoints;

public sealed class TestEndpointsStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/__test__/sign-in", async (
                    SignInRequest req,
                    AppDbContext db,
                    HttpContext http,
                    IIdentityResolver identtityResolver) =>
                {

                    var authUser = new FrankAuthCallbackResult
                    {
                        SubjectId = req.Sub,
                        GivenName = "Test",
                        FamilyName = "User",
                        Email = "test.user@campfitfurdogs.com",
                        Claims = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>())
                    };
                    var userId = await identtityResolver.ResolveAsync(authUser, CancellationToken.None);

                    // 3. Issue cookie
                    var claims = new Claim[]
                    {
                        new("sub", req.Sub),
                        new(ClaimTypes.NameIdentifier, userId.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, "Test");
                    var principal = new ClaimsPrincipal(identity);

                    await http.SignInAsync("cfd.session", principal);

                    // 4. Return internal IDs
                    return Results.Json(new
                    {
                        CustomerId = userId
                    });
                });
            });

            app.Map("/__test__/current-user-id", builder =>
            {
                builder.Run(async context =>
                {
                    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    await context.Response.WriteAsJsonAsync(new { userId });
                });
            });

            next(app);


        };
    }

    private sealed record SignInRequest(string Sub);
}
