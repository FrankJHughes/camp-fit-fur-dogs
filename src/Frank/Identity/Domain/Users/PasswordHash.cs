using Frank.Core.Domain;
using Frank.Identity.Domain.Users.Exceptions;

namespace Frank.Identity.Domain.Users;

/// <summary>
/// Represents a validated and normalized BCrypt password hash within the
/// Identity domain.
/// <para>
/// This value object ensures that only properly formatted BCrypt hashes enter
/// the user model, preventing malformed or insecure password representations.
/// </para>
/// <para>
/// The domain enforces the following invariants:
/// </para>
/// <list type="bullet">
/// <item><description>
/// The hash must be non‑empty and non‑whitespace.
/// </description></item>
/// <item><description>
/// The hash must begin with a valid BCrypt version prefix:
/// <c>$2a$</c>, <c>$2b$</c>, or <c>$2y$</c>.
/// </description></item>
/// <item><description>
/// Plaintext passwords are only permitted inside <see cref="Create"/> and
/// <see cref="Verify"/>.
/// </description></item>
/// </list>
/// </summary>
public sealed class PasswordHash : ValueObject
{
    /// <summary>
    /// Gets the stored BCrypt hash value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new <see cref="PasswordHash"/> instance, enforcing BCrypt
    /// prefix validation.
    /// </summary>
    /// <param name="value">The raw BCrypt hash string.</param>
    /// <exception cref="InvalidPasswordHashException">
    /// Thrown when the hash is empty or does not begin with a valid BCrypt
    /// version prefix.
    /// </exception>
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
    /// Wraps an existing BCrypt hash into a <see cref="PasswordHash"/> value object.
    /// </summary>
    /// <param name="hash">The raw BCrypt hash string.</param>
    /// <returns>A validated <see cref="PasswordHash"/> instance.</returns>
    public static PasswordHash From(string hash) => new(hash);

    /// <summary>
    /// Creates a new BCrypt hash from a plaintext password.
    /// <para>
    /// This is the <strong>only</strong> location in the domain where plaintext
    /// passwords are permitted. All other layers must operate exclusively on
    /// hashed values.
    /// </para>
    /// </summary>
    /// <param name="plaintext">The plaintext password.</param>
    /// <returns>A new <see cref="PasswordHash"/> containing the hashed password.</returns>
    /// <exception cref="InvalidPasswordHashException">
    /// Thrown when the plaintext password is empty or whitespace.
    /// </exception>
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
    /// <param name="plaintext">The plaintext password to verify.</param>
    /// <returns><c>true</c> if the password matches; otherwise <c>false</c>.</returns>
    public bool Verify(string plaintext) =>
        BCrypt.Net.BCrypt.Verify(plaintext, Value);

    /// <summary>
    /// Defines the components used to determine equality between instances.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the stored BCrypt hash string.
    /// </summary>
    public override string ToString() => Value;
}
