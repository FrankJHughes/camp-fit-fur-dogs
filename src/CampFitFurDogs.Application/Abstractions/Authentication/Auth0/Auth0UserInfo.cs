namespace CampFitFurDogs.Application.Abstractions.Authentication.Auth0;

public record Auth0UserInfo(
    string ExternalId,
    string FirstName,
    string LastName,
    string Email);
