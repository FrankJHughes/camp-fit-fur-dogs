namespace CampFitFurDogs.Application.Abstractions.Authentication;

public sealed record AuthCallbackResult(
    Guid CustomerId,
    string SessionCookie,
    string RedirectUrl
)
{
    public static AuthCallbackResult CreateSuccess(Guid customerId)
        => new(customerId, $"cfd.session={customerId}", "");

    public AuthCallbackResult WithRedirect(string redirectUrl)
        => this with { RedirectUrl = redirectUrl };
}
