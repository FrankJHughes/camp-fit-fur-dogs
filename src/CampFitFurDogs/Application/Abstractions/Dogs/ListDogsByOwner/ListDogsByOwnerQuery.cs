using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

public record ListDogsByOwnerQuery(Guid OwnerId) : IQuery<ListDogsByOwnerResponse>;
