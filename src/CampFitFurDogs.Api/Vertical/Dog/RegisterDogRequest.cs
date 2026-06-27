namespace CampFitFurDogs.Api.Vertical.Dog;

public sealed record RegisterDogRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
