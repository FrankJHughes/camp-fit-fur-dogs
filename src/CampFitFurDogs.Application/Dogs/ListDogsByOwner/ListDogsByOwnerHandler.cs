using CampFitFurDogs.Application.Abstractions.Dog.ListDogsByOwner;
using Frank.Abstractions.Query;

namespace CampFitFurDogs.Application.Dogs.ListDogsByOwner;

public sealed class ListDogsByOwnerHandler(IListDogsByOwnerReader reader)
    : IQueryHandler<ListDogsByOwnerQuery, ListDogsByOwnerResponse>
{
    public async Task<ListDogsByOwnerResponse> HandleAsync(
        ListDogsByOwnerQuery query, CancellationToken ct)
        => await reader.ListDogsByOwnerAsync(query.OwnerId, ct);
}
