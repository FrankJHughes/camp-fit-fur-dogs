using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Domain.Dogs;

public sealed class Breed : ValueObject
{
    public string Value { get; }

    private Breed(string value) => Value = value;

    public static Breed Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Breed is required.", nameof(value));
        return new Breed(value.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
