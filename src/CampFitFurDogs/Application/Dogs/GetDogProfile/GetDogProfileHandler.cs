using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace CampFitFurDogs.Application.Dogs.GetDogProfile;

public sealed class GetDogProfileHandler(IGetDogProfileReader reader)
    : IQueryHandler<GetDogProfileQuery, GetDogProfileResponse?>
{
    public async Task<GetDogProfileResponse?> HandleAsync(
        GetDogProfileQuery query, CancellationToken ct)
        => await reader.ReadAsync(query.DogId, query.OwnerId, ct);
}
