using CampFitFurDogs.Domain.Customers.Exceptions;
using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class PasswordHash : ValueObject
{
    public string Value { get; }

    private PasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidPasswordHashException("Password hash cannot be empty.");

        // Domain invariant: must be a valid BCrypt hash prefix
        if (!value.StartsWith("$2a$") &&
            !value.StartsWith("$2b$") &&
            !value.StartsWith("$2y$"))
        {
            throw new InvalidPasswordHashException("Password hash must be a valid BCrypt hash.");
        }

        Value = value;
    }

    /// <summary>
    /// Wraps an existing BCrypt hash into a PasswordHash value object.
    /// </summary>
    public static PasswordHash From(string hash) => new(hash);

    /// <summary>
    /// Creates a new BCrypt hash from a plaintext password.
    /// This is the ONLY place plaintext passwords are allowed.
    /// </summary>
    public static PasswordHash Create(string plaintext)
    {
        if (string.IsNullOrWhiteSpace(plaintext))
            throw new InvalidPasswordHashException("Password cannot be empty.");

        var hashed = BCrypt.Net.BCrypt.HashPassword(plaintext);
        return new PasswordHash(hashed);
    }

    /// <summary>
    /// Verifies a plaintext password against the stored BCrypt hash.
    /// </summary>
    public bool Verify(string plaintext) =>
        BCrypt.Net.BCrypt.Verify(plaintext, Value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
