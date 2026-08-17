using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using Frank.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Dogs;

/// <summary>
/// Infrastructure‑layer reader responsible for retrieving all dogs owned by a
/// specific user.
/// <para>
/// This reader implements <see cref="IListDogsByOwnerReader"/> and provides the
/// persistence behavior required by the application layer’s vertical slice for
/// listing dogs by owner.
/// </para>
/// <para>
/// The reader performs a no‑tracking query because the results are consumed in a
/// read‑only context and do not require change tracking.
/// </para>
/// </summary>
public sealed class ListDogsByOwnerReader : IListDogsByOwnerReader
{
    /// <summary>
    /// The EF Core database context used to query persisted aggregates.
    /// </summary>
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListDogsByOwnerReader"/> class.
    /// </summary>
    /// <param name="db">The application's database context.</param>
    public ListDogsByOwnerReader(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves all dogs belonging to the specified owner.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Converts the raw owner GUID into a <see cref="UserId"/>.</description></item>
    /// <item><description>Executes a no‑tracking query for read‑only consumption.</description></item>
    /// <item><description>Projects each <see cref="Dog"/> aggregate into a lightweight <see cref="DogSummary"/> DTO.</description></item>
    /// <item><description>Wraps the results in a <see cref="ListDogsByOwnerResponse"/>.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="ownerId">The raw GUID representing the owner identifier.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>
    /// A <see cref="ListDogsByOwnerResponse"/> containing summaries of all dogs
    /// owned by the specified user.
    /// </returns>
    public async Task<ListDogsByOwnerResponse> ReadAsync(Guid ownerId, CancellationToken ct)
    {
        var dogs = await _db.Set<Dog>()
            .AsNoTracking()
            .Where(d => d.OwnerId == UserId.From(ownerId))
            .Select(d => new DogSummary(d.Id.Value, d.Name.Value, d.Breed.Value))
            .ToListAsync(ct);

        return new ListDogsByOwnerResponse(dogs);
    }
}
