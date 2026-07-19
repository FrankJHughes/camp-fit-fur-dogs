using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;

public record GetDogProfileQuery(Guid DogId, Guid OwnerId) : IQuery<GetDogProfileResponse?>;
