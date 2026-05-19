using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class PasswordHash : ValueObject
{
    public string Value { get; }

    private PasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidPasswordHashException("Password hash cannot be empty.");

        if (!value.StartsWith("$2a$") &&
            !value.StartsWith("$2b$") &&
            !value.StartsWith("$2y$"))
        {
            throw new InvalidPasswordHashException("Password hash must be a valid BCrypt hash.");
        }

        Value = value;
    }

    public static PasswordHash From(string hash) => new(hash);

    public static PasswordHash Create(string plaintext)
    {
        if (string.IsNullOrWhiteSpace(plaintext))
            throw new InvalidPasswordHashException("Password cannot be empty.");

        var hashed = BCrypt.Net.BCrypt.HashPassword(plaintext);
        return new PasswordHash(hashed);
    }

    public bool Verify(string plaintext) =>
        BCrypt.Net.BCrypt.Verify(plaintext, Value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

// public sealed class InvalidPasswordHashException : DomainException
// {
//     public InvalidPasswordHashException(string message) : base(message) { }
// }
