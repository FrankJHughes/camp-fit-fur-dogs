using SharedKernel.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;

namespace CampFitFurDogs.Application.Dogs.GetDogProfile;

public sealed class GetDogProfileHandler(IGetDogProfileReader reader)
    : IQueryHandler<GetDogProfileQuery, GetDogProfileResponse?>
{
    public async Task<GetDogProfileResponse?> HandleAsync(
        GetDogProfileQuery query, CancellationToken ct)
        => await reader.GetDogProfileAsync(query.DogId, query.CustomerId, ct);
}
