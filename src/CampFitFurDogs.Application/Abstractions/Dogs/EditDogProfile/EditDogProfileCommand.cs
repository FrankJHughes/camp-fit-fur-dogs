using SharedKernel.Abstractions;

namespace CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;

public sealed record EditDogProfileCommand(
    Guid DogId,
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex) : ICommand;
