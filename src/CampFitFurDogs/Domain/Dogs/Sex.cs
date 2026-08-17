namespace CampFitFurDogs.Domain.Dogs;

/// <summary>
/// Represents the biological sex of a dog.
/// <para>
/// <see cref="Sex"/> is modeled as an enum because the domain defines a small,
/// fixed set of valid values that do not require additional behavior or
/// invariants.
/// </para>
/// <para>
/// This value is used by the <see cref="Dog"/> aggregate to capture immutable
/// biological characteristics of the animal.
/// </para>
/// </summary>
public enum Sex
{
    /// <summary>
    /// Indicates that the dog is male.
    /// </summary>
    Male,

    /// <summary>
    /// Indicates that the dog is female.
    /// </summary>
    Female
}
