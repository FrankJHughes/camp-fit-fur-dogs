using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty");

        Value = value.Trim();
    }

    public static PhoneNumber From(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
