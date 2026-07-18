using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Frank.Identity.Application.Abstractions;
using Frank.Core.Application.Abstractions.UnitOfWork;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Application.Abstractions.Authentication;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.Domain.Users;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Microsoft.AspNetCore.Mvc;
using Frank.Identity.Application.Abstractions.Sessions.CreateSession;

namespace CampFitFurDogs.TestUtilities.Endpoints.SignIn;

public sealed partial class SignInEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/__test__/sign-in", async (
            SignInRequest req,
            HttpContext http,
            [FromServices] IIdentityResolver identityResolver,
            [FromServices] ISessionTokenService sessionTokenService,
            [FromServices] ICreateSessionWriter sessionWriter,
            [FromServices] IFrankIdentityUnitOfWork unitOfWork) =>
        {
            // 1. Build a fake Frank authentication callback result
            var callback = new CallbackOidcContextBuilderResult
            {
                SubjectId = req.Sub,
                GivenName = "Test",
                FamilyName = "User",
                Email = "test.user@example.com",
                Claims = new ReadOnlyDictionary<string, string>(
                    new Dictionary<string, string>())
            };

            // 2. Resolve identity → UserId
            var ownerId = await identityResolver.ResolveAsync(callback, http.RequestAborted);

            // 3. Generate secure random plaintext token (256-bit)
            // 4. Hash for DB storage (SHA-256 hex)
            var sessionToken = sessionTokenService.Generate();

            // 5. Create a real session aggregate
            var session = Session.Create(
                tokenHash: sessionToken.HashedToken,
                ownerId: UserId.From(ownerId),
                createdAt: DateTimeOffset.UtcNow);

            // 6. Persist the session
            await sessionWriter.WriteAsync(session, http.RequestAborted);
            await unitOfWork.CommitAsync(http.RequestAborted);

            // 7. Write the plaintext token to the cookie
            http.Response.Cookies.Append(
                "session",
                sessionToken.PlaintextToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/"
                });

            // 8. Return identity + session info
            var response = new SignInResponse(
                OwnerId: ownerId,
                SessionId: session.Id.Value,
                PlaintextToken: sessionToken.PlaintextToken);

            return Results.Ok(response);
        })
        .AllowAnonymous();
    }

}
