using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Authentication.Sessions;

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
