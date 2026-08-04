using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace CampFitFurDogs.Application.Dogs.GetDog;

public sealed class GetDogHandler(IGetDogReader reader)
    : IQueryHandler<GetDogQuery, GetDogResponse?>
{
    public async Task<GetDogResponse?> HandleAsync(
        GetDogQuery query, CancellationToken ct)
        => await reader.ReadAsync(query.DogId, query.OwnerId, ct);
}
