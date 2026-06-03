using CampFitFurDogs.Domain.Authentication.Sessions;

namespace CampFitFurDogs.Application.Abstractions.Authentication;

public sealed record AuthCallbackContext(
    string Code,
    AuthToken? Token = null,
    AuthUser? User = null,
    Guid? CustomerId = null,
    SessionTokenHash? TokenHash = null,
    Session? Session = null,
    SessionCookie? SessionCookie = null,
    string? RedirectUrl = null,
    DateTimeOffset Now = default
)
{
    // ------------------------------------------------------------
    // Sequencing validation (NOT domain validation)
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
