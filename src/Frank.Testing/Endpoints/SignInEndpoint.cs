using System.Collections.ObjectModel;
using System.Security.Claims;
using Frank.Abstractions;
using Frank.Abstractions.Authentication.Callback;
using Frank.Abstractions.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.Testing.Endpoints;

public sealed class SignInEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/__test__/sign-in", async (
            SignInRequest req,
            HttpContext http,
            IIdentityResolver identityResolver) =>
        {
            var authUser = new FrankAuthCallbackResult
            {
                SubjectId = req.Sub,
                GivenName = "Test",
                FamilyName = "User",
                Email = "test.user@example.com",
                Claims = new ReadOnlyDictionary<string, string>(
                    new Dictionary<string, string>())
            };

            var userId = await identityResolver.ResolveAsync(authUser, CancellationToken.None);

            var claims = new[]
            {
                new Claim("sub", req.Sub),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            // Authentication type is irrelevant for cookies — scheme controls the handler
            var identity = new ClaimsIdentity(claims, req.Scheme);
            var principal = new ClaimsPrincipal(identity);

            await http.SignInAsync(req.Scheme, principal);

            return Results.Json(new { UserId = userId });
        });
    }

    public sealed record SignInRequest(string Sub, string Scheme);
}
