namespace CampFitFurDogs.Api.Dogs;

public sealed record RegisterDogRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
