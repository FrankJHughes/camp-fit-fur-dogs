using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Dogs.GetDogProfile;

public sealed class GetDogProfileHandler : IQueryHandler<GetDogProfileQuery, DogProfileResponse>
{
    private readonly IDogRepository _dogRepository;

    public GetDogProfileHandler(IDogRepository dogRepository)
    {
        _dogRepository = dogRepository;
    }

    public async Task<DogProfileResponse> Handle(GetDogProfileQuery query, CancellationToken ct)
    {
        var dog = await _dogRepository.GetByIdAsync(DogId.From(query.DogId), ct);

        if (dog is null)
            throw new KeyNotFoundException($"Dog {query.DogId} not found.");

        if (dog.OwnerId.Value != query.CustomerId)
            throw new UnauthorizedAccessException();

        return new DogProfileResponse(
            dog.Id.Value,
            dog.OwnerId.Value,
            dog.Name.Value,
            dog.Breed.Value,
            dog.DateOfBirth,
            dog.Sex.ToString());
    }
}
