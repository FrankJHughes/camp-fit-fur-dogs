using Frank.Core.Domain;

namespace CampFitFurDogs.Domain.Dogs;

/// <summary>
/// Represents the unique identifier for a <see cref="Dog"/> aggregate.
/// <para>
/// <see cref="DogId"/> is a strongly‑typed wrapper around <see cref="Guid"/>,
/// ensuring type safety across the domain and preventing accidental misuse of
/// raw GUID values.
/// </para>
/// <para>
/// Instances are created either via <see cref="New"/> for new aggregates or
/// <see cref="From(Guid)"/> when rehydrating existing ones.
/// </para>
/// </summary>
public sealed class DogId : AggregateId
{
    /// <summary>
    /// Initializes a new <see cref="DogId"/> instance with the specified GUID.
    /// <para>
    /// This constructor is private to ensure all instances pass through
    /// <see cref="New"/> or <see cref="From(Guid)"/>, which enforce domain
    /// invariants.
    /// </para>
    /// </summary>
    /// <param name="value">The underlying GUID value.</param>
    private DogId(Guid value) : base(value) { }

    /// <summary>
    /// Creates a new <see cref="DogId"/> using a freshly generated GUID.
    /// <para>
    /// This method is used when creating new <see cref="Dog"/> aggregates.
    /// </para>
    /// </summary>
    /// <returns>A new, unique <see cref="DogId"/> instance.</returns>
    public static DogId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a <see cref="DogId"/> from an existing GUID value.
    /// <para>
    /// This method is used when rehydrating aggregates from persistence.
    /// </para>
    /// </summary>
    /// <param name="value">The GUID value representing the dog identifier.</param>
    /// <returns>A <see cref="DogId"/> instance wrapping the provided GUID.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>, which
    /// is not a valid aggregate identifier.
    /// </exception>
    public static DogId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("DogId cannot be empty.", nameof(value));

        return new DogId(value);
    }
}
