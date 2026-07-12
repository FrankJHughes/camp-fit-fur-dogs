namespace CampFitFurDogs.Api.Endpoints.Dog;

public sealed record RegisterDogRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
