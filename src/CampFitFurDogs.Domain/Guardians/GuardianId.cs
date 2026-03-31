namespace CampFitFurDogs.Domain.Guardians;

using CampFitFurDogs.SharedKernel;

public sealed class GuardianId : ValueObject
{
    public Guid Value { get; }

    private GuardianId(Guid value) => Value = value;

    public static GuardianId New() => new(Guid.NewGuid());

    public static GuardianId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(value));

        return new(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
