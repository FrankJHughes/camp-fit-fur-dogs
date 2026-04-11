using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Infrastructure.Dogs;

public sealed class DogRepository : IDogRepository
{
    private readonly AppDbContext _db;

    public DogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Dog dog, CancellationToken cancellationToken = default)
    {
        await _db.Dogs.AddAsync(dog, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Dog?> GetByIdAsync(DogId id, CancellationToken cancellationToken = default)
    {
        return await _db.Dogs.FindAsync([id, cancellationToken], cancellationToken);
    }

}
