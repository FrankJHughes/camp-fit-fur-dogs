using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace CampFitFurDogs.Application.Abstractions.Dogs.EditDog;

public sealed record EditDogCommand(
    Guid DogId,
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex) : ICommand;
