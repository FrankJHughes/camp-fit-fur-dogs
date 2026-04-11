using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Application.Dogs.GetDogProfile;

public record GetDogProfileQuery(Guid DogId, Guid CustomerId) : IQuery<DogProfileResponse>;
