using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace CampFitFurDogs.Application.Dogs.ListDogsByOwner;

public sealed class ListDogsByOwnerHandler(IListDogsByOwnerReader reader)
    : IQueryHandler<ListDogsByOwnerQuery, ListDogsByOwnerResponse>
{
    public async Task<ListDogsByOwnerResponse> HandleAsync(
        ListDogsByOwnerQuery query, CancellationToken ct)
        => await reader.ReadAsync(query.OwnerId, ct);
}
