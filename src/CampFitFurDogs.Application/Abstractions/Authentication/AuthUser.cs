namespace CampFitFurDogs.Application.Abstractions.Authentication;

public sealed record AuthUser(
    string ExternalId,
    string GivenName,
    string FamilyName,
    string Email
);
