using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using Frank.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Dogs;

public sealed class ListDogsByOwnerReader(AppDbContext db) : IListDogsByOwnerReader
{
    public async Task<ListDogsByOwnerResponse> ReadAsync(
        Guid ownerId, CancellationToken ct)
    {
        var dogs = await db.Set<Dog>()
            .AsNoTracking()
            .Where(d => d.OwnerId == UserId.From(ownerId))
            .Select(d => new DogSummary(d.Id.Value, d.Name.Value, d.Breed.Value))
            .ToListAsync(ct);

        return new ListDogsByOwnerResponse(dogs);
    }
}
