using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;

namespace CampFitFurDogs.Application.Authentication;

public sealed class AuthCallbackContext
{
    public string Code { get; }

    public AuthToken? Token { get; set; }
    public AuthUser? User { get; set; }

    public Guid? CustomerId { get; set; }
    public SessionTokenHash? TokenHash { get; set; }
    public Session? Session { get; set; }
    public SessionCookie? SessionCookie { get; set; }

    public string? RedirectUrl { get; set; }

    /// <summary>
    /// Timestamp captured at the start of the callback flow.
    /// Used for session creation and auditing.
    /// </summary>
    public DateTimeOffset Now { get; set; }

    public AuthCallbackContext(string code)
    {
        Code = code;
    }

    // ------------------------------------------------------------
    // Pipeline sequencing validation (NOT domain validation)
    // ------------------------------------------------------------

    public void RequireCode()
    {
        if (string.IsNullOrWhiteSpace(Code))
            throw new InvalidOperationException("AuthCallbackContext: Code must be present before this step.");
    }

    public void RequireToken()
    {
        if (Token is null)
            throw new InvalidOperationException("AuthCallbackContext: Token must be set before this step.");
    }

    public void RequireUser()
    {
        if (User is null)
            throw new InvalidOperationException("AuthCallbackContext: User must be set before this step.");
    }

    public void RequireCustomerId()
    {
        if (CustomerId is null)
            throw new InvalidOperationException("AuthCallbackContext: CustomerId must be set before this step.");
    }

    public void RequireTokenHash()
    {
        if (TokenHash is null)
            throw new InvalidOperationException("AuthCallbackContext: TokenHash must be set before this step.");
    }

    public void RequireSession()
    {
        if (Session is null)
            throw new InvalidOperationException("AuthCallbackContext: Session must be created before this step.");
    }

    public void RequireSessionCookie()
    {
        if (SessionCookie is null)
            throw new InvalidOperationException("AuthCallbackContext: SessionCookie must be set before this step.");
    }

    public void RequireRedirectUrl()
    {
        if (string.IsNullOrWhiteSpace(RedirectUrl))
            throw new InvalidOperationException("AuthCallbackContext: RedirectUrl must be set before this step.");
    }
}
