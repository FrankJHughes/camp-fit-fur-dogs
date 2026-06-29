using CampFitFurDogs.Application.Abstractions.Dog.GetDogProfile;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Dogs;

public sealed class GetDogProfileReader(AppDbContext db) : IGetDogProfileReader
{
    public async Task<GetDogProfileResponse?> GetDogProfileAsync(
        Guid dogId, Guid ownerId, CancellationToken ct)
    {
        var dog = await db.Set<Dog>()
            .Where(d =>
                d.OwnerId == CustomerId.From(ownerId) &&
                d.Id == DogId.From(dogId))
            .SingleOrDefaultAsync(ct);

        if (dog is null)
        {
            return null;
        }

        return new GetDogProfileResponse(
            dog.Id.Value,
            dog.OwnerId.Value,
            dog.Name.Value,
            dog.Breed.Value,
            dog.DateOfBirth,
            dog.Sex.ToString());
    }
}
