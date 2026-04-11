using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Domain.Dogs;

public sealed class DogName : ValueObject
{
    public string Value { get; }

    private DogName(string value) => Value = value;

    public static DogName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Dog name is required.", nameof(value));
        return new DogName(value.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
