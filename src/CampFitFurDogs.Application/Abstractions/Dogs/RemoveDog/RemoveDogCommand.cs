using Frank.Abstractions;

namespace CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;

public sealed record RemoveDogCommand(
    Guid DogId,
    Guid OwnerId) : ICommand;
