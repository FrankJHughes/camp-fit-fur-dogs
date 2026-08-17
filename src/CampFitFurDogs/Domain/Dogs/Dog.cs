using Frank.Core.Domain;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Domain.Dogs;

/// <summary>
/// Represents a dog owned by a user within the CampFitFurDogs domain.
/// <para>
/// <see cref="Dog"/> is an aggregate root responsible for maintaining the
/// invariants and lifecycle of a dog entity, including its identity, ownership,
/// name, breed, date of birth, and sex.
/// </para>
/// <para>
/// All mutations occur through well‑defined methods (<see cref="Create"/>,
/// <see cref="Update"/>) to ensure domain consistency.
/// </para>
/// </summary>
public sealed class Dog : AggregateRoot<DogId>
{
    /// <summary>
    /// Gets the identifier of the owner who owns this dog.
    /// <para>
    /// Ownership is immutable after creation and represents a core domain
    /// invariant: a dog always belongs to exactly one owner.
    /// </para>
    /// </summary>
    public UserId OwnerId { get; private set; } = default!;

    /// <summary>
    /// Gets the dog's name as a domain value object.
    /// <para>
    /// <see cref="DogName"/> encapsulates validation and formatting rules for
    /// dog names.
    /// </para>
    /// </summary>
    public DogName Name { get; private set; } = default!;

    /// <summary>
    /// Gets the dog's breed as a domain value object.
    /// </summary>
    public Breed Breed { get; private set; } = default!;

    /// <summary>
    /// Gets the dog's date of birth.
    /// </summary>
    public DateOnly DateOfBirth { get; private set; }

    /// <summary>
    /// Gets the dog's biological sex.
    /// </summary>
    public Sex Sex { get; private set; }

    /// <summary>
    /// Required by EF Core and other ORMs for materialization.
    /// </summary>
    private Dog() { }

    /// <summary>
    /// Initializes a new <see cref="Dog"/> aggregate with the specified values.
    /// <para>
    /// This constructor is private to ensure that all creation flows pass
    /// through <see cref="Create"/>, which enforces domain invariants and
    /// generates a new <see cref="DogId"/>.
    /// </para>
    /// </summary>
    /// <param name="id">The unique identifier of the dog.</param>
    /// <param name="ownerId">The identifier of the owner.</param>
    /// <param name="name">The dog's name.</param>
    /// <param name="breed">The dog's breed.</param>
    /// <param name="dateOfBirth">The dog's date of birth.</param>
    /// <param name="sex">The dog's sex.</param>
    private Dog(DogId id, UserId ownerId, DogName name, Breed breed, DateOnly dateOfBirth, Sex sex)
        : base(id)
    {
        OwnerId = ownerId;
        Name = name;
        Breed = breed;
        DateOfBirth = dateOfBirth;
        Sex = sex;
    }

    /// <summary>
    /// Creates a new <see cref="Dog"/> aggregate instance.
    /// <para>
    /// This factory method ensures that a new <see cref="DogId"/> is generated
    /// and that all domain invariants are satisfied at creation time.
    /// </para>
    /// </summary>
    /// <param name="ownerId">The identifier of the owner who will own the dog.</param>
    /// <param name="name">The dog's name.</param>
    /// <param name="breed">The dog's breed.</param>
    /// <param name="dateOfBirth">The dog's date of birth.</param>
    /// <param name="sex">The dog's sex.</param>
    /// <returns>A fully initialized <see cref="Dog"/> aggregate.</returns>
    public static Dog Create(UserId ownerId, DogName name, Breed breed, DateOnly dateOfBirth, Sex sex)
    {
        return new Dog(DogId.New(), ownerId, name, breed, dateOfBirth, sex);
    }

    /// <summary>
    /// Updates the mutable properties of the dog.
    /// <para>
    /// This method represents the domain‑approved mutation path for updating
    /// a dog's profile. Ownership cannot be changed after creation.
    /// </para>
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="breed">The new breed.</param>
    /// <param name="dateOfBirth">The new date of birth.</param>
    /// <param name="sex">The new sex.</param>
    public void Update(DogName name, Breed breed, DateOnly dateOfBirth, Sex sex)
    {
        Name = name;
        Breed = breed;
        DateOfBirth = dateOfBirth;
        Sex = sex;
    }
}
