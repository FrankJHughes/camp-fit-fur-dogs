using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Infrastructure.Dogs;

public sealed class GetDogProfileReader(AppDbContext db) : IGetDogProfileReader
{
    public async Task<DogProfileResponse?> GetDogProfileAsync(
        Guid dogId, Guid ownerId, CancellationToken ct)
    {
        var dog = await db.Set<Dog>().FindAsync([DogId.From(dogId)], ct);

        if (dog is null || dog.OwnerId.Value != ownerId)
            return null;

        return new DogProfileResponse(
            dog.Id.Value,
            dog.OwnerId.Value,
            dog.Name.Value,
            dog.Breed.Value,
            dog.DateOfBirth,
            dog.Sex.ToString());
    }
}
