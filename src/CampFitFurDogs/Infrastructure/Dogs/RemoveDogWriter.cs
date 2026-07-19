using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;

namespace CampFitFurDogs.Infrastructure.Dogs;

public sealed class RemoveDogWriter : IRemoveDogWriter
{
    private readonly AppDbContext _db;

    public RemoveDogWriter(AppDbContext db)
    {
        _db = db;
    }

    public async Task WriteAsync(Guid dogId, CancellationToken cancellationToken = default)
    {
        var dog = _db.Set<Dog>().SingleOrDefault(d => d.Id == DogId.From(dogId)) ??
            throw new InvalidOperationException($"Dog {dogId} not found.");
        _db.Set<Dog>().Remove(dog);
    }
}
