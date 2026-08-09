using System.Text.RegularExpressions;
using Frank.Core.Domain;
using Frank.Identity.Domain.Sessions.Errors;

namespace Frank.Identity.Domain.Sessions;

/// <summary>
/// Represents the SHA‑256 hash of a session token.
/// <para>
/// This value object enforces strict formatting rules to ensure that only
/// valid, non‑empty, 64‑character hexadecimal SHA‑256 hashes can enter the
/// domain. The raw plaintext token is never stored server‑side; only its
/// hash is persisted for lookup and validation.
/// </para>
/// <para>
/// The type provides factory methods, parsing helpers, and equality semantics
/// consistent with other Identity domain primitives.
/// </para>
/// </summary>
public sealed class SessionTokenHash : ValueObject
{
    /// <summary>
    /// Strict SHA‑256 hex format: exactly 64 hexadecimal characters
    /// (uppercase or lowercase).
    /// </summary>
    private static readonly Regex Sha256HexRegex =
        new("^[A-Fa-f0-9]{64}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the underlying SHA‑256 hex string value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new <see cref="SessionTokenHash"/> with the specified value.
    /// <para>
    /// Domain rule: The value must be a valid 64‑character SHA‑256 hex string.
    /// </para>
    /// </summary>
    /// <param name="value">The SHA‑256 hex string.</param>
    /// <exception cref="InvalidSessionTokenHashException">
    /// Thrown when the value is null, empty, whitespace, or not a valid SHA‑256 hex string.
    /// </exception>
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
    /// Creates a <see cref="SessionTokenHash"/> from a raw SHA‑256 hex string,
    /// enforcing all domain invariants.
    /// </summary>
    /// <param name="value">The SHA‑256 hex string.</param>
    /// <returns>A new <see cref="SessionTokenHash"/> instance.</returns>
    public static SessionTokenHash From(string value)
        => new(value);

    /// <summary>
    /// Attempts to create a <see cref="SessionTokenHash"/> from a raw string.
    /// <para>
    /// Returns <c>true</c> if the value is a valid SHA‑256 hex string; otherwise <c>false</c>.
    /// </para>
    /// </summary>
    /// <param name="value">The raw string to validate.</param>
    /// <param name="hash">
    /// The resulting <see cref="SessionTokenHash"/> if valid; otherwise <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the value is valid; otherwise <c>false</c>.
    /// </returns>
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
    /// Attempts to parse a raw string into a <see cref="SessionTokenHash"/>.
    /// <para>
    /// This is an alias for <see cref="TryFrom(string?, out SessionTokenHash?)"/>
    /// to maintain API symmetry with other domain primitives.
    /// </para>
    /// </summary>
    /// <param name="value">The raw string to parse.</param>
    /// <param name="hash">
    /// The resulting <see cref="SessionTokenHash"/> if valid; otherwise <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the value is valid; otherwise <c>false</c>.
    /// </returns>
    public static bool TryParse(string? value, out SessionTokenHash? hash)
        => TryFrom(value, out hash);

    /// <summary>
    /// Defines the components used to determine equality between instances.
    /// </summary>
    /// <returns>
    /// The sequence of components that uniquely identify this value object.
    /// </returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the underlying SHA‑256 hex string.
    /// </summary>
    public override string ToString() => Value;
}
