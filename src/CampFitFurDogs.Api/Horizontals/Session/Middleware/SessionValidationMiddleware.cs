using System.Security.Claims;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Customer.GetCustomerById;
using CampFitFurDogs.Application.Abstractions.Sessions.GetSession;
using CampFitFurDogs.Application.Settings;
using CampFitFurDogs.Domain.Sessions.Errors;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.Api.Horizontals.Session.Middleware;

public sealed class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;

    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context,
        IGetCustomerByIdReader getCustomerReader,
        IGetSessionReader getSessionReader,
        ISessionTokenService tokens,
        IOptionsMonitor<SessionSettings> sessionOptions)
    {
        var excludes = new string[]
        {
            "/api/identity/callback",
            "/api/identity/login",
            "/api/identity/logout"
        };

        if (excludes.Any(exclude => context.Request.Path.StartsWithSegments(exclude)))
        {
            await _next(context);
            return;
        }

        var ttl = sessionOptions.CurrentValue.Ttl;

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
        var session = await getSessionReader.GetSessionAsync(tokenHash, context.RequestAborted);

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


        var customer = await getCustomerReader.GetByIdAsync(session.OwnerId, context.RequestAborted);
        if (customer is null)
        {
            context.Response.Cookies.Delete("session");
            throw new SessionNotFoundException();
        }

        var customerName = $"{customer.FirstName} {customer.LastName}";
        // 5. Attach authenticated owner to HttpContext
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, session.OwnerId.ToString()),
                new Claim(ClaimTypes.Name, customerName)
            ],
            authenticationType: "Session");

        context.User = new ClaimsPrincipal(identity);
        context.Items["CurrentOwnerId"] = session.OwnerId;

        // 6. Continue pipeline
        await _next(context);
    }
}
