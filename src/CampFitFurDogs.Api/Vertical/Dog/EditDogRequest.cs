namespace CampFitFurDogs.Api.Vertical.Dog;

public sealed record EditDogProfileRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
