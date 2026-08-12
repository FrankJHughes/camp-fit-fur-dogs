using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;

namespace CampFitFurDogs.Infrastructure.Dogs;

/// <summary>
/// Infrastructure‑layer writer responsible for persisting a newly created
/// <see cref="Dog"/> aggregate.
/// <para>
/// This writer implements <see cref="IRegisterDogWriter"/> and provides the
/// persistence behavior required by the application layer’s vertical slice for
/// registering a new dog.
/// </para>
/// <para>
/// The writer does not save changes directly; committing the transaction is
/// handled by the application layer’s unit‑of‑work or pipeline behavior.
/// </para>
/// </summary>
public sealed class RegisterDogWriter : IRegisterDogWriter
{
    /// <summary>
    /// The EF Core database context used to access persisted aggregates.
    /// </summary>
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterDogWriter"/> class.
    /// </summary>
    /// <param name="db">The application's database context.</param>
    public RegisterDogWriter(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Persists a newly created <see cref="Dog"/> aggregate.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Adds the dog to the EF Core change tracker.</description></item>
    /// <item><description>Defers saving changes to the application layer.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="dog">The newly created <see cref="Dog"/> aggregate.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public async Task WriteAsync(Dog dog, CancellationToken cancellationToken = default)
    {
        await _db.Set<Dog>().AddAsync(dog, cancellationToken);
    }
}
