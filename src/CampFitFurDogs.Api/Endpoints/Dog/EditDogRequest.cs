namespace CampFitFurDogs.Api.Endpoints.Dog;

public sealed record EditDogProfileRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
