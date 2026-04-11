using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Domain.Dogs;

public sealed class DogId : ValueObject
{
    public Guid Value { get; }

    private DogId(Guid value) => Value = value;

    public static DogId New() => new(Guid.NewGuid());

    public static DogId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("DogId cannot be empty.", nameof(value));
        return new DogId(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
