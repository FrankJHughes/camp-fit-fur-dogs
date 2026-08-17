using Frank.Core.Domain;
using Frank.Identity.Domain.Users.Exceptions;

namespace Frank.Identity.Domain.Users;

/// <summary>
/// Represents an external authentication provider identifier in the form
/// <c>"provider|identifier"</c>.
/// <para>
/// This value object enforces strict formatting rules to ensure that external
/// identity references are always well‑formed, normalized, and unambiguous.
/// </para>
/// <para>
/// The format consists of two non‑empty parts:
/// </para>
/// <list type="bullet">
/// <item><description><strong>provider</strong> — the external authentication provider name (e.g., <c>auth0</c>, <c>google</c>, <c>azuread</c>)</description></item>
/// <item><description><strong>identifier</strong> — the provider‑specific user identifier (e.g., subject ID)</description></item>
/// </list>
/// <para>
/// These parts are separated by a single pipe character (<c>|</c>), forming a
/// stable composite identity key such as <c>"auth0|abc123"</c>.
/// </para>
/// </summary>
public sealed class ExternalId : ValueObject
{
    /// <summary>
    /// Gets the normalized external identity value in the form
    /// <c>"provider|identifier"</c>.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new <see cref="ExternalId"/> instance, enforcing the
    /// required <c>"provider|identifier"</c> format.
    /// </summary>
    /// <param name="value">The raw external identity string.</param>
    /// <exception cref="InvalidExternalAuthProviderIdException">
    /// Thrown when the value is empty, whitespace, or does not conform to the
    /// required two‑segment pipe‑delimited format.
    /// </exception>
    private ExternalId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidExternalAuthProviderIdException("External auth provider ID cannot be empty.");

        value = value.Trim();

        // Domain invariant: must be in the form "provider|identifier"
        var parts = value.Split('|', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
            throw new InvalidExternalAuthProviderIdException(
                $"External auth provider ID must be in the format 'provider|id'. Received: '{value}'.");

        if (string.IsNullOrWhiteSpace(parts[0]))
            throw new InvalidExternalAuthProviderIdException("External auth provider name cannot be empty.");

        if (string.IsNullOrWhiteSpace(parts[1]))
            throw new InvalidExternalAuthProviderIdException("External auth provider user ID cannot be empty.");

        Value = value;
    }

    /// <summary>
    /// Creates a new <see cref="ExternalId"/> instance from the specified raw
    /// string, enforcing all domain invariants.
    /// </summary>
    /// <param name="value">The raw external identity string.</param>
    /// <returns>A validated <see cref="ExternalId"/> instance.</returns>
    public static ExternalId From(string value) => new(value);

    /// <summary>
    /// Defines the components used to determine equality between instances.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the normalized external identity string.
    /// </summary>
    public override string ToString() => Value;
}
