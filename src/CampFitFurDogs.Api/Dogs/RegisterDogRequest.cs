namespace CampFitFurDogs.Api.Dogs;

public sealed record RegisterDogRequest(
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex);
