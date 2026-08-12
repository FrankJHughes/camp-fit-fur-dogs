using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using Frank.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Dogs;

/// <summary>
/// Infrastructure‑layer writer responsible for updating an existing
/// <see cref="Dog"/> aggregate in the database.
/// <para>
/// This writer implements <see cref="IEditDogWriter"/>, providing the
/// persistence behavior required by the application layer’s vertical slice
/// for editing dog profiles.
/// </para>
/// <para>
/// The writer ensures that:
/// <list type="bullet">
/// <item><description>The dog exists.</description></item>
/// <item><description>The dog belongs to the requesting owner.</description></item>
/// <item><description>The aggregate is updated using domain‑approved mutation.</description></item>
/// </list>
/// </para>
/// </summary>
public sealed class EditDogWriter : IEditDogWriter
{
    /// <summary>
    /// The EF Core database context used to access persisted aggregates.
    /// </summary>
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditDogWriter"/> class.
    /// </summary>
    /// <param name="db">The application's database context.</param>
    public EditDogWriter(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Updates an existing <see cref="Dog"/> aggregate with new values.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Loads the dog by its <see cref="DogId"/>.</description></item>
    /// <item><description>Ensures the dog belongs to the specified <see cref="UserId"/> owner.</description></item>
    /// <item><description>Applies domain‑approved updates via <see cref="Dog.Update"/>.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="ownerId">The owner requesting the update.</param>
    /// <param name="id">The identifier of the dog to update.</param>
    /// <param name="name">The new dog name.</param>
    /// <param name="breed">The new breed.</param>
    /// <param name="dateOfBirth">The new date of birth.</param>
    /// <param name="sex">The new biological sex.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the dog does not exist or does not belong to the specified owner.
    /// </exception>
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
