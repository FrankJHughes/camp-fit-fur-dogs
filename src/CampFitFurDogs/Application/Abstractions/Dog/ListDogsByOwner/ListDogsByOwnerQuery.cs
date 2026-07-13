using Frank.Core.Application.Abstractions.Query;

namespace CampFitFurDogs.Application.Abstractions.Dog.ListDogsByOwner;

public record ListDogsByOwnerQuery(Guid OwnerId) : IQuery<ListDogsByOwnerResponse>;
