using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;

namespace CampFitFurDogs.Application.Dogs.GetDogProfile;

public sealed class GetDogProfileHandler(IGetDogProfileReader reader)
    : IQueryHandler<GetDogProfileQuery, DogProfileResponse?>
{
    public async Task<DogProfileResponse?> Handle(
        GetDogProfileQuery query, CancellationToken ct)
        => await reader.GetDogProfileAsync(query.DogId, query.CustomerId, ct);
}
