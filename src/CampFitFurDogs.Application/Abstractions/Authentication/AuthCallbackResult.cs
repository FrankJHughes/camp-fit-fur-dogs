using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Abstractions.Authentication;

public sealed record AuthCallbackResult(
    CustomerId CustomerId,
    SessionCookie Cookie,
    string RedirectUrl
)
{
    public AuthCallbackResult WithRedirect(string redirectUrl)
        => this with { RedirectUrl = redirectUrl };
}
