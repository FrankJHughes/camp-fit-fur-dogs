using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CampFitFurDogs.Domain.Customers.Exceptions;
using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class FirstName : ValueObject
{
    // Allow letters, spaces, hyphens, and apostrophes.
    // This supports names like:
    // - O'Connor
    // - Jean-Luc
    // - Mary Ann
    // - D’Angelo (after normalization)
    private static readonly Regex ValidChars = new(
        @"^[A-Za-z' -]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private FirstName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidFirstNameException("First name cannot be empty.");

        // Normalize to canonical Unicode form
        value = value.Trim().Normalize(NormalizationForm.FormC);

        // Domain invariant: enforce reasonable length
        if (value.Length is < 1 or > 100)
            throw new InvalidFirstNameException("First name must be between 1 and 100 characters.");

        // Domain invariant: enforce allowed characters
        if (!ValidChars.IsMatch(value))
            throw new InvalidFirstNameException("First name contains invalid characters.");

        Value = value;
    }

    public static FirstName From(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
