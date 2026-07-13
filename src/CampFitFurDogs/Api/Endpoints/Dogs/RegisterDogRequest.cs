namespace CampFitFurDogs.Api.Endpoints.Dogs;

public sealed record RegisterDogRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
