using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Domain.Customers;

public sealed class PasswordHash : ValueObject
{
    public string Value { get; }

    private PasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Password hash cannot be empty");

        Value = value;
    }

    public static PasswordHash From(string value) => new(value);

    // BCrypt with default work factor 11 (2^11 = 2048 iterations).
    // Chosen for simplicity, automatic salting, and adaptive cost.
    public static PasswordHash Create(string plaintext)
    {
        if (string.IsNullOrWhiteSpace(plaintext))
            throw new ArgumentException("Password cannot be empty.", nameof(plaintext));

        var hashed = BCrypt.Net.BCrypt.HashPassword(plaintext);
        return new PasswordHash(hashed);
    }

    public bool Verify(string plaintext)
    {
        return BCrypt.Net.BCrypt.Verify(plaintext, Value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
