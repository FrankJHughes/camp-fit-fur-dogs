using Frank.Abstractions;

namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;

public record GetDogProfileQuery(Guid DogId, Guid CustomerId) : IQuery<GetDogProfileResponse?>;
