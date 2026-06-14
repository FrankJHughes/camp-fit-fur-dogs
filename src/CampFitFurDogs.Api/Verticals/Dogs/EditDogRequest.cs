namespace CampFitFurDogs.Api.Verticals.Dogs;

public sealed record EditDogProfileRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
