#nullable enable
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Frank.Identity.Api.Middleware.Sessions;

/// <summary>
/// Authentication handler for the Identity API’s session‑based authentication scheme.
/// <para>
/// This handler does not perform any validation itself.
/// Instead, it simply returns the authenticated principal that was previously
/// established by <c>SessionValidationMiddleware</c>.
/// If no principal has been set, the handler reports <see cref="AuthenticateResult.NoResult"/>.
/// </para>
/// </summary>
/// <remarks>
/// This class exists to integrate the Identity session model with ASP.NET Core’s
/// authentication infrastructure.
/// It ensures that:
/// <list type="bullet">
/// <item><description>The “Session” authentication scheme participates in the pipeline.</description></item>
/// <item><description>Authorization policies can rely on <c>Context.User</c> being populated.</description></item>
/// <item><description>No session validation logic is duplicated here.</description></item>
/// <item><description>Purity is preserved — validation belongs in the session middleware, not the handler.</description></item>
/// </list>
/// </remarks>
public sealed class SessionAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    /// <summary>
    /// Initializes the handler with the required ASP.NET Core infrastructure services.
    /// </summary>
    /// <param name="options">Monitors authentication scheme options.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The URL encoder.</param>
    public SessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    /// <summary>
    /// Returns the authenticated session principal if one has already been set by
    /// <c>SessionValidationMiddleware</c>.
    /// <para>
    /// This handler does not validate cookies, tokens, or session state.
    /// It simply reflects the authentication state already present in
    /// <see cref="HttpContext.User"/>.
    /// </para>
    /// </summary>
    /// <returns>
    /// <see cref="AuthenticateResult.Success"/> if the user is authenticated;
    /// otherwise <see cref="AuthenticateResult.NoResult"/>.
    /// </returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // If SessionValidationMiddleware already set an authenticated principal,
        // return it as the result of AuthenticateAsync("Session").
        if (Context.User?.Identity?.IsAuthenticated == true)
        {
            var ticket = new AuthenticationTicket(Context.User, "Session");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
