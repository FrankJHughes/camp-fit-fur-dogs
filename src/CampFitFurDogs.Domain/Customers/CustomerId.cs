using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Domain.Customers;

public sealed class CustomerId : ValueObject
{
    public Guid Value { get; }

    private CustomerId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty");

        Value = value;
    }

    public static CustomerId New() => new(Guid.NewGuid());
    public static CustomerId From(Guid value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
