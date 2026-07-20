using System.Security.Claims;
using Frank.Identity.Application.Abstractions.Sessions;
using Frank.Identity.Application.Abstractions.Sessions.GetSession;
using Frank.Identity.Application.Abstractions.Users.GetUserById;
using Frank.Identity.Domain.Sessions.Errors;
using Microsoft.AspNetCore.Http;

namespace Frank.Identity.Api.Middleware.Sessions;

public sealed class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;

    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context,
        IGetUserByIdReader getUserReader,
        IGetSessionReader getSessionReader,
        ISessionTokenService tokens)
    {
        var excludes = new string[]
        {
            "/api/identity/callback",
            "/api/identity/login-url",
            "/api/identity/logout"
        };

        if (excludes.Any(exclude => context.Request.Path.StartsWithSegments(exclude)))
        {
            await _next(context);
            return;
        }

        // 1. Read plaintext token from cookie
        var plaintextToken = context.Request.Cookies["session"];

        if (string.IsNullOrWhiteSpace(plaintextToken))
        {
            await _next(context);
            return;
        }

        // 2. Hash the plaintext token
        string tokenHash;
        try
        {
            tokenHash = tokens.Hash(plaintextToken).Value;
        }
        catch
        {
            context.Response.Cookies.Delete("session");
            await _next(context);
            return;
        }

        // 3. Load session by token hash
        var session = await getSessionReader.ReadAsync(tokenHash, context.RequestAborted);

        if (session is null)
        {
            context.Response.Cookies.Delete("session");
            throw new SessionNotFoundException();
        }

        // 4. Validate domain invariants
        var now = DateTimeOffset.UtcNow;

        if (session.IsRevoked || session.IsExpired)
        {
            context.Response.Cookies.Delete("session");
            throw new SessionNotFoundException();
        }


        var user = await getUserReader.ReadAsync(session.OwnerId, context.RequestAborted);
        if (user is null)
        {
            context.Response.Cookies.Delete("session");
            throw new SessionNotFoundException();
        }

        var userName = $"{user.FirstName} {user.LastName}";
        // 5. Attach authenticated owner to HttpContext
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, session.OwnerId.ToString()),
                new Claim(ClaimTypes.Name, userName)
            ],
            authenticationType: "Session");

        context.User = new ClaimsPrincipal(identity);
        context.Items["CurrentOwnerId"] = session.OwnerId;

        // 6. Continue pipeline
        await _next(context);
    }
}
