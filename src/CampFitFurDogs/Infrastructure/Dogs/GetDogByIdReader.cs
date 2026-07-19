using CampFitFurDogs.Application.Abstractions.Dogs.GetDogById;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Dogs;

public sealed class GetDogByIdReader(AppDbContext db) : IGetDogByIdReader
{
    public async Task<Dog?> ReadAsync(
        Guid dogId, CancellationToken ct)
    {
        var dog = await db.Set<Dog>()
            .Where(d => d.Id == DogId.From(dogId))
            .SingleOrDefaultAsync(ct);

        return dog;
    }
}
