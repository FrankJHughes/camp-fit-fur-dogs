using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using Frank.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Dogs;

public sealed class GetDogReader(AppDbContext db) : IGetDogReader
{
    public async Task<GetDogResponse?> ReadAsync(
        Guid dogId, Guid ownerId, CancellationToken ct)
    {
        var dog = await db.Set<Dog>()
            .AsNoTracking()
            .Where(d =>
                d.OwnerId == UserId.From(ownerId) &&
                d.Id == DogId.From(dogId))
            .SingleOrDefaultAsync(ct);

        if (dog is null)
        {
            return null;
        }

        return new GetDogResponse(
            dog.Id.Value,
            dog.OwnerId.Value,
            dog.Name.Value,
            dog.Breed.Value,
            dog.DateOfBirth,
            dog.Sex.ToString());
    }
}
