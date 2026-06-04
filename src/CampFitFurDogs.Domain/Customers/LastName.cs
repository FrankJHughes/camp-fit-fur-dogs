using System.Text;
using System.Text.RegularExpressions;
using CampFitFurDogs.Domain.Customers.Exceptions;
using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class LastName : ValueObject
{
    // Allow letters, spaces, hyphens, and apostrophes.
    // Supports names like:
    // - O'Connor
    // - Jean-Luc
    // - Van der Meer
    // - D’Angelo (after normalization)
    private static readonly Regex ValidChars = new(
        @"^[A-Za-z' -]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private LastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidLastNameException("Last name cannot be empty.");

        // Normalize to canonical Unicode form
        value = value.Trim().Normalize(NormalizationForm.FormC);

        // Domain invariant: enforce reasonable length
        if (value.Length is < 1 or > 100)
            throw new InvalidLastNameException("Last name must be between 1 and 100 characters.");

        // Domain invariant: enforce allowed characters
        if (!ValidChars.IsMatch(value))
            throw new InvalidLastNameException("Last name contains invalid characters.");

        Value = value;
    }

    public static LastName From(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
