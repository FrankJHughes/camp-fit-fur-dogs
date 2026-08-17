using Frank.Core.Domain;

namespace CampFitFurDogs.Domain.Dogs;

/// <summary>
/// Represents the breed of a dog as a domain value object.
/// <para>
/// <see cref="Breed"/> encapsulates the raw string value and ensures that
/// breed names are non‑empty, trimmed, and treated as immutable.
/// </para>
/// <para>
/// As a value object, equality is determined solely by the underlying
/// <see cref="Value"/>.
/// </para>
/// </summary>
public sealed class Breed : ValueObject
{
    /// <summary>
    /// Gets the normalized breed name.
    /// <para>
    /// This value is guaranteed to be non‑empty and trimmed, as enforced by
    /// <see cref="Create(string)"/>.
    /// </para>
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new <see cref="Breed"/> instance.
    /// <para>
    /// This constructor is private to ensure that all instances are created
    /// through <see cref="Create(string)"/>, which enforces domain invariants.
    /// </para>
    /// </summary>
    /// <param name="value">The normalized breed name.</param>
    private Breed(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="Breed"/> value object.
    /// <para>
    /// This factory method enforces the domain invariant that breed names
    /// must be non‑empty and non‑whitespace. The value is trimmed before
    /// being stored.
    /// </para>
    /// </summary>
    /// <param name="value">The raw breed name.</param>
    /// <returns>A validated and normalized <see cref="Breed"/> instance.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="value"/> is null, empty, or whitespace.
    /// </exception>
    public static Breed Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Breed is required.", nameof(value));

        return new Breed(value.Trim());
    }

    /// <summary>
    /// Gets the components used to determine equality between value objects.
    /// <para>
    /// For <see cref="Breed"/>, equality is based solely on the normalized
    /// <see cref="Value"/>.
    /// </para>
    /// </summary>
    /// <returns>An enumerable containing the breed value.</returns>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
