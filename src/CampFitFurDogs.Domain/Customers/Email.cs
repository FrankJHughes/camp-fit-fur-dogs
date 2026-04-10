namespace CampFitFurDogs.Domain.Customers;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty");

        value = value.Trim().ToLowerInvariant();

        if (!value.Contains("@"))
            throw new ArgumentException("Invalid email format");

        Value = value;
    }

    public static Email From(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
