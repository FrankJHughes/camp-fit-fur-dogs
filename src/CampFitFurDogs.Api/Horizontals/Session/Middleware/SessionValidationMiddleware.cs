using System.Security.Claims;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Sessions.GetSession;
using CampFitFurDogs.Application.Settings;
using CampFitFurDogs.Domain.Sessions.Errors;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.Api.Horizontals.Session.Middleware;

public sealed class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IGetSessionReader _reader;
    private readonly ISessionTokenService _tokens;
    private readonly TimeSpan _ttl;

    public SessionValidationMiddleware(
        RequestDelegate next,
        IGetSessionReader reader,
        ISessionTokenService tokens,
        IOptionsMonitor<SessionSettings> sessionOptions)
    {
        _next = next;
        _reader = reader;
        _tokens = tokens;
        _ttl = sessionOptions.CurrentValue.Ttl;
    }

    public async Task InvokeAsync(HttpContext context)
    {
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
            tokenHash = _tokens.Hash(plaintextToken).Value;
        }
        catch
        {
            context.Response.Cookies.Delete("session");
            await _next(context);
            return;
        }

        // 3. Load session by token hash
        var session = await _reader.GetSessionAsync(tokenHash, context.RequestAborted);

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

        // 5. Attach authenticated owner to HttpContext
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, session.OwnerId.ToString())
            },
            authenticationType: "Session");

        context.User = new ClaimsPrincipal(identity);
        context.Items["CurrentOwnerId"] = session.OwnerId;

        // 6. Continue pipeline
        await _next(context);
    }
}
