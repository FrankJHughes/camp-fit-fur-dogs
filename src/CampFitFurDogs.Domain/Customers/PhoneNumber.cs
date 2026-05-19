using System.Text.RegularExpressions;
using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex AllowedChars = new(
        @"^[0-9+\-\s().]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private PhoneNumber() { } // EF Core
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private PhoneNumber(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidPhoneNumberException("Phone number cannot be empty.");

        raw = raw.Trim();

        if (!AllowedChars.IsMatch(raw))
            throw new InvalidPhoneNumberException("Phone number contains invalid characters.");

        // Extract digits
        var digits = new string(raw.Where(char.IsDigit).ToArray());

        if (digits.Length < 10)
            throw new InvalidPhoneNumberException("Phone number must contain at least 10 digits.");

        if (digits.Length > 15)
            throw new InvalidPhoneNumberException("Phone number cannot exceed 15 digits.");

        // Normalize to E.164 (assume US if 10 digits)
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
}

// public sealed class InvalidPhoneNumberException : DomainException
// {
//     public InvalidPhoneNumberException(string message) : base(message) { }
// }
