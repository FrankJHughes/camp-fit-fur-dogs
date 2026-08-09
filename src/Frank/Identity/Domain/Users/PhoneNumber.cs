using System.Text.RegularExpressions;
using Frank.Core.Domain;
using Frank.Identity.Domain.Users.Exceptions;

namespace Frank.Identity.Domain.Users;

/// <summary>
/// Represents a normalized, E.164‑compatible phone number within the Identity domain.
/// <para>
/// This value object accepts common user‑entered formats (e.g., <c>(916) 555‑1234</c>,
/// <c>916‑555‑1234</c>, <c>+1 916 555 1234</c>) and normalizes them into a strict
/// E.164 representation (e.g., <c>+19165551234</c>).
/// </para>
/// <para>
/// Validation includes:
/// </para>
/// <list type="bullet">
/// <item><description>Non‑empty input</description></item>
/// <item><description>Allowed characters: digits, spaces, parentheses, hyphens, plus sign</description></item>
/// <item><description>Digit extraction and digit‑count enforcement (10–15 digits)</description></item>
/// <item><description>
/// Automatic country‑code inference for 10‑digit numbers (assumes US/Canada and prefixes <c>1</c>)
/// </description></item>
/// </list>
/// <para>
/// The final stored value is always normalized to E.164 format.
/// </para>
/// </summary>
public sealed class PhoneNumber : ValueObject
{
    /// <summary>
    /// Regular expression enforcing allowed characters for user‑entered phone numbers.
    /// </summary>
    private static readonly Regex AllowedChars = new(
        @"^[0-9+\-\s().]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Gets the normalized E.164 phone number value (e.g., <c>+19165551234</c>).
    /// </summary>
    public string Value { get; }

#pragma warning disable CS8618
    /// <summary>
    /// Parameterless constructor required by EF Core materialization.
    /// </summary>
    private PhoneNumber() { }
#pragma warning restore CS8618

    /// <summary>
    /// Initializes a new <see cref="PhoneNumber"/> instance, enforcing allowed
    /// characters, digit extraction, digit‑count rules, and E.164 normalization.
    /// </summary>
    /// <param name="raw">The raw phone number string.</param>
    /// <exception cref="InvalidPhoneNumberException">
    /// Thrown when the phone number is empty, contains invalid characters, or
    /// does not meet E.164 digit‑count requirements.
    /// </exception>
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
        // If 10 digits → assume US/Canada and prefix with country code 1
        if (digits.Length == 10)
            digits = "1" + digits;

        Value = "+" + digits;
    }

    /// <summary>
    /// Creates a new <see cref="PhoneNumber"/> instance from the specified raw
    /// string, enforcing all domain invariants.
    /// </summary>
    /// <param name="value">The raw phone number string.</param>
    /// <returns>A validated and normalized <see cref="PhoneNumber"/> instance.</returns>
    public static PhoneNumber From(string value) => new(value);

    /// <summary>
    /// Defines the components used to determine equality between instances.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the normalized E.164 phone number string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Returns <c>null</c> to represent an intentionally empty phone number.
    /// </summary>
    public static PhoneNumber? Empty() => null;
}
