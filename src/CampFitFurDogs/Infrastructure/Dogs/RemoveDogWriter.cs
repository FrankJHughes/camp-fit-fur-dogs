using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;

namespace CampFitFurDogs.Infrastructure.Dogs;

/// <summary>
/// Infrastructure‑layer writer responsible for removing an existing
/// <see cref="Dog"/> aggregate from persistence.
/// <para>
/// This writer implements <see cref="IRemoveDogWriter"/> and provides the
/// deletion behavior required by the application layer’s vertical slice for
/// removing a dog.
/// </para>
/// <para>
/// The writer does not commit changes directly; saving is handled by the
/// application layer’s unit‑of‑work or pipeline behavior.
/// </para>
/// </summary>
public sealed class RemoveDogWriter : IRemoveDogWriter
{
    /// <summary>
    /// The EF Core database context used to access persisted aggregates.
    /// </summary>
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveDogWriter"/> class.
    /// </summary>
    /// <param name="db">The application's database context.</param>
    public RemoveDogWriter(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Removes the specified <see cref="Dog"/> aggregate from persistence.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Converts the raw GUID into a <see cref="DogId"/>.</description></item>
    /// <item><description>Loads the dog from the database.</description></item>
    /// <item><description>Throws if the dog does not exist.</description></item>
    /// <item><description>Marks the dog for deletion.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="dogId">The raw GUID representing the dog identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the dog does not exist.
    /// </exception>
    public async Task WriteAsync(Guid dogId, CancellationToken cancellationToken = default)
    {
        var dog = _db.Set<Dog>().SingleOrDefault(d => d.Id == DogId.From(dogId))
            ?? throw new InvalidOperationException($"Dog {dogId} not found.");

        _db.Set<Dog>().Remove(dog);
    }
}
