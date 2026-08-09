#nullable enable
using System.Security.Claims;
using Frank.Identity.Application.Abstractions.Sessions;
using Frank.Identity.Application.Abstractions.Sessions.GetSession;
using Frank.Identity.Application.Abstractions.Users.GetUserById;
using Frank.Identity.Domain.Sessions.Errors;
using Microsoft.AspNetCore.Http;

namespace Frank.Identity.Api.Middleware.Sessions;

/// <summary>
/// Middleware responsible for validating the Identity API’s session cookie,
/// loading the associated session record, enforcing domain invariants, and
/// attaching an authenticated principal to <see cref="HttpContext.User"/>.
/// <para>
/// This middleware performs the *actual* session validation logic for the
/// “Session” authentication scheme.
/// The <see cref="SessionAuthenticationHandler"/> simply reflects the principal
/// created here.
/// </para>
/// </summary>
/// <remarks>
/// This middleware enforces the guarantees defined in the Identity session model:
/// <list type="bullet">
/// <item><description>Session cookies are hashed before lookup.</description></item>
/// <item><description>Revoked or expired sessions are rejected.</description></item>
/// <item><description>Missing or invalid sessions result in <see cref="SessionNotFoundException"/>.</description></item>
/// <item><description>User resolution is required; missing users invalidate the session.</description></item>
/// <item><description>No identity provider tokens or claims are exposed.</description></item>
/// </list>
/// It is intentionally placed in the Identity API assembly because it depends on
/// Identity abstractions such as <see cref="IGetSessionReader"/>,
/// <see cref="IGetUserByIdReader"/>, and <see cref="ISessionTokenGenerator"/>.
/// </remarks>
public sealed class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes the middleware with the next delegate in the pipeline.
    /// </summary>
    /// <param name="next">The next middleware component.</param>
    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Validates the session cookie, loads the associated session record, enforces
    /// domain invariants, resolves the user, and attaches an authenticated principal
    /// to the HTTP context.
    /// <para>
    /// The flow is:
    /// <list type="number">
    /// <item><description>Skip validation for known anonymous endpoints.</description></item>
    /// <item><description>Read plaintext session cookie.</description></item>
    /// <item><description>Hash the cookie value using <see cref="ISessionTokenGenerator"/>.</description></item>
    /// <item><description>Load session by hash.</description></item>
    /// <item><description>Validate domain invariants (revocation, expiration).</description></item>
    /// <item><description>Load the owning user.</description></item>
    /// <item><description>Create a <see cref="ClaimsPrincipal"/> for the authenticated user.</description></item>
    /// <item><description>Attach principal to <see cref="HttpContext.User"/>.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="getUserReader">Reader for loading user records.</param>
    /// <param name="getSessionReader">Reader for loading session records.</param>
    /// <param name="tokens">Token generator for hashing session cookies.</param>
    /// <exception cref="SessionNotFoundException">
    /// Thrown when the session is missing, revoked, expired, or associated with a
    /// non‑existent user.
    /// </exception>
    public async Task InvokeAsync(
        HttpContext context,
        IGetUserByIdReader getUserReader,
        IGetSessionReader getSessionReader,
        ISessionTokenGenerator tokens)
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
