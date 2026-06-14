namespace CampFitFurDogs.Api.Verticals.Dogs;

public sealed record RegisterDogRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
