namespace CampFitFurDogs.Api.Verticals.Dog;

public sealed record EditDogProfileRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
