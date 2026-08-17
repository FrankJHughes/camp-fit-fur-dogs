using System.Text;
using System.Text.RegularExpressions;
using Frank.Core.Domain;
using Frank.Identity.Domain.Users.Exceptions;

namespace Frank.Identity.Domain.Users;

/// <summary>
/// Represents a normalized, validated last name within the Identity domain.
/// <para>
/// This value object enforces strict formatting, character rules, and length
/// constraints to ensure that only well‑formed personal names enter the user
/// model.
/// </para>
/// <para>
/// Normalization includes:
/// </para>
/// <list type="bullet">
/// <item><description>Trimming surrounding whitespace</description></item>
/// <item><description>Normalizing to Unicode Form C</description></item>
/// </list>
/// <para>
/// Allowed characters include:
/// </para>
/// <list type="bullet">
/// <item><description>Alphabetic letters (A–Z, a–z)</description></item>
/// <item><description>Spaces</description></item>
/// <item><description>Hyphens (<c>-</c>)</description></item>
/// <item><description>Apostrophes (<c>'</c>)</description></item>
/// </list>
/// <para>
/// This supports names such as:
/// </para>
/// <list type="bullet">
/// <item><description>O'Connor</description></item>
/// <item><description>Jean‑Luc</description></item>
/// <item><description>Van der Meer</description></item>
/// <item><description>D’Angelo (after normalization)</description></item>
/// </list>
/// </summary>
public sealed class LastName : ValueObject
{
    /// <summary>
    /// Regular expression enforcing allowed characters for last names.
    /// </summary>
    private static readonly Regex ValidChars = new(
        @"^[A-Za-z' -]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Gets the normalized last name value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new <see cref="LastName"/> instance, enforcing normalization,
    /// length constraints, and allowed‑character rules.
    /// </summary>
    /// <param name="value">The raw last name string.</param>
    /// <exception cref="InvalidLastNameException">
    /// Thrown when the last name is empty, too long, too short, or contains
    /// invalid characters.
    /// </exception>
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

    /// <summary>
    /// Creates a new <see cref="LastName"/> instance from the specified raw
    /// string, enforcing all domain invariants.
    /// </summary>
    /// <param name="value">The raw last name string.</param>
    /// <returns>A validated and normalized <see cref="LastName"/> instance.</returns>
    public static LastName From(string value) => new(value);

    /// <summary>
    /// Defines the components used to determine equality between instances.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the normalized last name string.
    /// </summary>
    public override string ToString() => Value;
}
