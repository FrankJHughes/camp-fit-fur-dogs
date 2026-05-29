namespace CampFitFurDogs.Application.Authentication;

// ------------------------------------------------------------
// Value Objects
// ------------------------------------------------------------

public record Auth0Config(
    string Domain,
    string ClientId,
    string ClientSecret,
    string CallbackUrl,
    string PostLoginRedirectUrl);
