namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDog;

public record GetDogResponse(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex);
