using System.Text.RegularExpressions;
using CampFitFurDogs.Domain.Customers.Exceptions;
using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class PhoneNumber : ValueObject
{
    // Allow digits, spaces, parentheses, hyphens, and plus sign.
    // This allows common user-entered formats:
    // (916) 555-1234
    // 916-555-1234
    // +1 916 555 1234
    private static readonly Regex AllowedChars = new(
        @"^[0-9+\-\s().]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

#pragma warning disable CS8618
    private PhoneNumber() { } // EF Core
#pragma warning restore CS8618

    private PhoneNumber(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidPhoneNumberException("Phone number cannot be empty.");

        raw = raw.Trim();

        if (!AllowedChars.IsMatch(raw))
            throw new InvalidPhoneNumberException("Phone number contains invalid characters.");

        // Extract digits only
        var digits = new string(raw.Where(char.IsDigit).ToArray());

        // Domain invariants: enforce E.164-compatible digit count
        if (digits.Length < 10)
            throw new InvalidPhoneNumberException("Phone number must contain at least 10 digits.");

        if (digits.Length > 15)
            throw new InvalidPhoneNumberException("Phone number cannot exceed 15 digits.");

        // Normalize to E.164
        // If 10 digits → assume US and prefix with country code 1
        if (digits.Length == 10)
            digits = "1" + digits;

        Value = "+" + digits;
    }

    public static PhoneNumber From(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static PhoneNumber? Empty() => null;
}
