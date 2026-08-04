using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDog;

public record GetDogQuery(Guid DogId, Guid OwnerId) : IQuery<GetDogResponse?>;
