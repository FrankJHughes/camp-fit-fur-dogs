using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;

public sealed record RegisterDogCommand(
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex) : ICommand<Guid>;
