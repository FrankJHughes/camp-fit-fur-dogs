using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using Frank.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Dogs;

public sealed class EditDogWriter : IEditDogWriter
{
    private readonly AppDbContext _db;

    public EditDogWriter(AppDbContext db)
    {
        _db = db;
    }

    public async Task WriteAsync(
        UserId ownerId,
        DogId id,
        DogName name,
        Breed breed,
        DateOnly dateOfBirth,
        Sex sex,
        CancellationToken cancellationToken = default)
    {
        var existingDog = await _db.Set<Dog>()
            .SingleOrDefaultAsync(d =>
                d.Id == id, cancellationToken);

        if (existingDog is null || !existingDog.OwnerId.Value.Equals(ownerId.Value))
            throw new InvalidOperationException("Dog not found.");

        existingDog.Update(name, breed, dateOfBirth, sex);
    }
}
