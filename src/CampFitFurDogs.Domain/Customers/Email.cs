using System.Text.RegularExpressions;
using CampFitFurDogs.Domain.Customers.Exceptions;
using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed partial class Email : ValueObject
{
    private static readonly Regex EmailRegex = EmailPattern();

    public string Value { get; }

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

    public static Email From(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    /// <summary>
    /// Practical, strict-enough email validation:
    /// - No leading dot
    /// - Local part: alphanumeric + ._%+-
    /// - Domain: labels cannot start with hyphen
    /// - TLD: 2–63 letters
    /// </summary>
    [GeneratedRegex(
        @"^(?!\.)[A-Za-z0-9._%+-]+@(?!-)([A-Za-z0-9-]+\.)+[A-Za-z]{2,63}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex EmailPattern();
}
