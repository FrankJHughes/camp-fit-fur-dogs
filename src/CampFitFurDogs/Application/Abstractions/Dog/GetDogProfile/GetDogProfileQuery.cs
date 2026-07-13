using Frank.Core.Application.Abstractions.Query;

namespace CampFitFurDogs.Application.Abstractions.Dog.GetDogProfile;

public record GetDogProfileQuery(Guid DogId, Guid OwnerId) : IQuery<GetDogProfileResponse?>;
