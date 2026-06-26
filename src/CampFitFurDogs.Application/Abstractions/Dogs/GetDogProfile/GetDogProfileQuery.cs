using Frank.Abstractions.Query;

namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;

public record GetDogProfileQuery(Guid DogId, Guid OwnerId) : IQuery<GetDogProfileResponse?>;
