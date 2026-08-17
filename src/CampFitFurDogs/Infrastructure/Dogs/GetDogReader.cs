using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using Frank.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Dogs;

/// <summary>
/// Infrastructure‑layer reader responsible for retrieving a single dog profile
/// for a specific owner.
/// <para>
/// This reader implements <see cref="IGetDogReader"/> and provides the
/// persistence behavior required by the application layer’s
/// <see cref="GetDogResponse"/> vertical slice.
/// </para>
/// <para>
/// The reader performs a no‑tracking query because the result is consumed
/// in a read‑only context.
/// </para>
/// </summary>
public sealed class GetDogReader : IGetDogReader
{
    /// <summary>
    /// The EF Core database context used to query persisted aggregates.
    /// </summary>
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDogReader"/> class.
    /// </summary>
    /// <param name="db">The application's database context.</param>
    public GetDogReader(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves a dog profile for the specified owner.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Converts raw GUIDs into <see cref="DogId"/> and <see cref="UserId"/>.</description></item>
    /// <item><description>Executes a no‑tracking query for read‑only consumption.</description></item>
    /// <item><description>Ensures the dog belongs to the requesting owner.</description></item>
    /// <item><description>Maps the domain aggregate into a <see cref="GetDogResponse"/> DTO.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="dogId">The raw GUID representing the dog identifier.</param>
    /// <param name="ownerId">The raw GUID representing the owner identifier.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>
    /// A <see cref="GetDogResponse"/> containing the dog's profile,
    /// or <c>null</c> if the dog does not exist or does not belong to the owner.
    /// </returns>
    public async Task<GetDogResponse?> ReadAsync(
        Guid dogId, Guid ownerId, CancellationToken ct)
    {
        var dog = await _db.Set<Dog>()
            .AsNoTracking()
            .Where(d =>
                d.OwnerId == UserId.From(ownerId) &&
                d.Id == DogId.From(dogId))
            .SingleOrDefaultAsync(ct);

        if (dog is null)
        {
            return null;
        }

        return new GetDogResponse(
            dog.Id.Value,
            dog.OwnerId.Value,
            dog.Name.Value,
            dog.Breed.Value,
            dog.DateOfBirth,
            dog.Sex.ToString());
    }
}
