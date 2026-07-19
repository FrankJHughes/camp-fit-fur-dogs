using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;

namespace CampFitFurDogs.Infrastructure.Dogs;

public sealed class RegisterDogWriter : IRegisterDogWriter
{
    private readonly AppDbContext _db;

    public RegisterDogWriter(AppDbContext db)
    {
        _db = db;
    }

    public async Task WriteAsync(Dog dog, CancellationToken cancellationToken = default)
    {
        await _db.Set<Dog>().AddAsync(dog, cancellationToken);
    }
}
