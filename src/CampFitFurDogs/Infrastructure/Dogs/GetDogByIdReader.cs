using CampFitFurDogs.Application.Abstractions.Dogs.GetDogById;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Dogs;

/// <summary>
/// Infrastructure‑layer reader responsible for retrieving a <see cref="Dog"/>
/// aggregate by its identifier.
/// <para>
/// This reader implements <see cref="IGetDogByIdReader"/> and provides the
/// persistence behavior required by the application layer’s vertical slice for
/// fetching a single dog profile.
/// </para>
/// <para>
/// The reader performs a no‑tracking query because the application layer
/// consumes the result in a read‑only context.
/// </para>
/// </summary>
public sealed class GetDogByIdReader : IGetDogByIdReader
{
    /// <summary>
    /// The EF Core database context used to query persisted aggregates.
    /// </summary>
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDogByIdReader"/> class.
    /// </summary>
    /// <param name="db">The application's database context.</param>
    public GetDogByIdReader(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves a <see cref="Dog"/> aggregate by its identifier.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Converts the raw <see cref="Guid"/> into a <see cref="DogId"/>.</description></item>
    /// <item><description>Executes a no‑tracking query for read‑only consumption.</description></item>
    /// <item><description>Returns <c>null</c> if the dog does not exist.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="dogId">The raw GUID representing the dog identifier.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>
    /// The matching <see cref="Dog"/> aggregate, or <c>null</c> if no match is found.
    /// </returns>
    public async Task<Dog?> ReadAsync(Guid dogId, CancellationToken ct)
    {
        var dog = await _db.Set<Dog>()
            .AsNoTracking()
            .Where(d => d.Id == DogId.From(dogId))
            .SingleOrDefaultAsync(ct);

        return dog;
    }
}
