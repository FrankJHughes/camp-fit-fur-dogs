using Frank.Abstractions.Command;

namespace CampFitFurDogs.Application.Abstractions.Dog.RemoveDog;

public sealed record RemoveDogCommand(
    Guid DogId,
    Guid OwnerId) : ICommand;
