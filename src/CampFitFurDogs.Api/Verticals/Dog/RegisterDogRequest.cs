namespace CampFitFurDogs.Api.Verticals.Dog;

public sealed record RegisterDogRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
