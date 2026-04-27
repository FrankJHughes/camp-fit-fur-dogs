using Microsoft.EntityFrameworkCore;
using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Infrastructure.Dogs;

public sealed class ListDogsByOwnerReader(AppDbContext db) : IListDogsByOwnerReader
{
    public async Task<ListDogsByOwnerResponse> ListDogsByOwnerAsync(
        Guid ownerId, CancellationToken ct)
    {
        var dogs = await db.Set<Dog>()
            .Where(d => d.OwnerId == CustomerId.From(ownerId))
            .Select(d => new DogSummary(d.Id.Value, d.Name.Value, d.Breed.Value))
            .ToListAsync(ct);

        return new ListDogsByOwnerResponse(dogs);
    }
}
