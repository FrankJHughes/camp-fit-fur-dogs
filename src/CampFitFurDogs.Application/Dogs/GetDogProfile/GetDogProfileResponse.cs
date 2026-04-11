namespace CampFitFurDogs.Application.Dogs.GetDogProfile;

public record DogProfileResponse(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex);
