using System.Text.RegularExpressions;
using Frank.Core.Domain;
using Frank.Identity.Domain.Users.Exceptions;

namespace Frank.Identity.Domain.Users;

/// <summary>
/// Represents a normalized, validated email address within the Identity domain.
/// <para>
/// This value object enforces strict syntactic validation, normalization rules,
/// and domain invariants to ensure that only well‑formed email addresses can
/// enter the user model.
/// </para>
/// <para>
/// Normalization includes:
/// </para>
/// <list type="bullet">
/// <item><description>Trimming surrounding whitespace</description></item>
/// <item><description>Converting to lowercase</description></item>
/// </list>
/// <para>
/// Validation includes:
/// </para>
/// <list type="bullet">
/// <item><description>No leading dot in the local part</description></item>
/// <item><description>Local part allows alphanumeric + ._%+-</description></item>
/// <item><description>Domain labels cannot start with a hyphen</description></item>
/// <item><description>TLD must be 2–63 alphabetic characters</description></item>
/// </list>
/// </summary>
public sealed partial class Email : ValueObject
{
    /// <summary>
    /// Compiled regular expression used to validate email format according to
    /// the domain’s strict-but-practical rules.
    /// </summary>
    private static readonly Regex EmailRegex = EmailPattern();

    /// <summary>
    /// Gets the normalized email value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new <see cref="Email"/> instance, enforcing normalization
    /// and syntactic validation rules.
    /// </summary>
    /// <param name="value">The raw email string.</param>
    /// <exception cref="InvalidEmailException">
    /// Thrown when the email is empty, whitespace, or fails validation.
    /// </exception>
    private Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException("Email cannot be empty.");

        // Normalize
        value = value.Trim().ToLowerInvariant();

        // Domain invariant: must be a syntactically valid email
        if (!EmailRegex.IsMatch(value))
            throw new InvalidEmailException($"Invalid email format: '{value}'.");

        Value = value;
    }

    /// <summary>
    /// Creates a new <see cref="Email"/> instance from the specified raw string,
    /// enforcing all domain invariants.
    /// </summary>
    /// <param name="value">The raw email string.</param>
    /// <returns>A validated and normalized <see cref="Email"/> instance.</returns>
    public static Email From(string value) => new(value);

    /// <summary>
    /// Defines the components used to determine equality between instances.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the normalized email string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Practical, strict-enough email validation:
    /// <list type="bullet">
    /// <item><description>No leading dot</description></item>
    /// <item><description>Local part: alphanumeric + ._%+-</description></item>
    /// <item><description>Domain labels cannot start with hyphen</description></item>
    /// <item><description>TLD: 2–63 letters</description></item>
    /// </list>
    /// </summary>
    [GeneratedRegex(
        @"^(?!\.)[A-Za-z0-9._%+-]+@(?!-)([A-Za-z0-9-]+\.)+[A-Za-z]{2,63}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex EmailPattern();
}
