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

    public async Task AddAsync(Domain.Dogs.Dog dog, CancellationToken cancellationToken = default)
    {
        await _db.Set<Domain.Dogs.Dog>().AddAsync(dog, cancellationToken);
    }

    public async Task<Domain.Dogs.Dog?> GetByIdAsync(DogId id, CancellationToken cancellationToken = default)
    {
        return await _db.Set<Domain.Dogs.Dog>().FindAsync(new object[] { id }, cancellationToken);
    }

    public Task DeleteAsync(Domain.Dogs.Dog dog, CancellationToken cancellationToken = default)
    {
        _db.Set<Domain.Dogs.Dog>().Remove(dog);
        return Task.CompletedTask;
    }
}
