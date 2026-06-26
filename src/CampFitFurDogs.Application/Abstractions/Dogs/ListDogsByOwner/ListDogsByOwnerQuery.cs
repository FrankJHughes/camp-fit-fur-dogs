using Frank.Abstractions.Query;

namespace CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

public record ListDogsByOwnerQuery(Guid OwnerId) : IQuery<ListDogsByOwnerResponse>;
