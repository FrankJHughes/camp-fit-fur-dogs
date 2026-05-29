namespace CampFitFurDogs.Application.Abstractions.Authentication;

// ------------------------------------------------------------
// Value Objects
// ------------------------------------------------------------

public record AuthCallbackResult(Guid CustomerId, string RedirectUrl);
