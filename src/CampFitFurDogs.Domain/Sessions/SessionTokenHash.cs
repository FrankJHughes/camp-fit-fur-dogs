using System.Text.RegularExpressions;
using CampFitFurDogs.Domain.Sessions.Errors;
using Frank.Domain;

namespace CampFitFurDogs.Domain.Sessions;

public sealed class SessionTokenHash : ValueObject
{
    // Strict SHA‑256 hex format (64 lowercase or uppercase hex chars)
    private static readonly Regex Sha256HexRegex =
        new("^[A-Fa-f0-9]{64}$", RegexOptions.Compiled);

    public string Value { get; }

    private SessionTokenHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidSessionTokenHashException("Token hash cannot be empty.");

        if (!Sha256HexRegex.IsMatch(value))
            throw new InvalidSessionTokenHashException(
                "Token hash must be a 64‑character SHA‑256 hex string.");

        Value = value;
    }

    /// <summary>
    /// Creates a SessionTokenHash, enforcing domain invariants.
    /// </summary>
    public static SessionTokenHash From(string value)
        => new(value);

    /// <summary>
    /// Attempts to create a SessionTokenHash from a string.
    /// Returns true if valid, false otherwise.
    /// </summary>
    public static bool TryFrom(string? value, out SessionTokenHash? hash)
    {
        hash = null;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (!Sha256HexRegex.IsMatch(value))
            return false;

        hash = new SessionTokenHash(value);
        return true;
    }

    /// <summary>
    /// Attempts to parse a string into a SessionTokenHash.
    /// Alias for TryFrom for API symmetry.
    /// </summary>
    public static bool TryParse(string? value, out SessionTokenHash? hash)
        => TryFrom(value, out hash);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
