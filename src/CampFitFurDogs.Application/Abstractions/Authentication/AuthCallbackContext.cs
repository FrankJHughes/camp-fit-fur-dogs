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

    public DateTimeOffset CreatedAt { get; set; }

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
            throw new InvalidOperationException("Authorization code must be present.");
    }

    public void RequireToken()
    {
        if (Token is null)
            throw new InvalidOperationException("Auth token must be set before this step.");
    }

    public void RequireUser()
    {
        if (User is null)
            throw new InvalidOperationException("Auth user profile must be set before this step.");
    }

    public void RequireCustomerId()
    {
        if (CustomerId is null)
            throw new InvalidOperationException("CustomerId must be set before this step.");
    }

    public void RequireTokenHash()
    {
        if (TokenHash is null)
            throw new InvalidOperationException("TokenHash must be set before this step.");
    }

    public void RequireSession()
    {
        if (Session is null)
            throw new InvalidOperationException("Session must be created before this step.");
    }

    public void RequireSessionCookie()
    {
        if (SessionCookie is null)
            throw new InvalidOperationException("SessionCookie must be set before this step.");
    }

    public void RequireRedirectUrl()
    {
        if (RedirectUrl is null)
            throw new InvalidOperationException("RedirectUrl must be set before this step.");
    }
}
