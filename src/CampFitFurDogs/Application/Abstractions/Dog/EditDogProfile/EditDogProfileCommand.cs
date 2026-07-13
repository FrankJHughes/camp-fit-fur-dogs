using Frank.Core.Application.Abstractions.Command;

namespace CampFitFurDogs.Application.Abstractions.Dog.EditDogProfile;

public sealed record EditDogProfileCommand(
    Guid DogId,
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex) : ICommand;
