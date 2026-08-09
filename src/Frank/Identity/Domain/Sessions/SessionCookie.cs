using Frank.Core.Domain;

namespace Frank.Identity.Domain.Sessions;

/// <summary>
/// Represents the cookie used to store a plaintext session token on the client.
/// <para>
/// This value object encapsulates the cookie name and value, ensuring consistent
/// formatting and equality semantics across the Identity subsystem.
/// </para>
/// <para>
/// The cookie value is the *plaintext* session token issued to the client.
/// The server stores only the hashed form (<see cref="SessionTokenHash"/>),
/// but the client must retain the plaintext token for subsequent authentication.
/// </para>
/// </summary>
public sealed class SessionCookie : ValueObject
{
    /// <summary>
    /// Gets the name of the cookie.
    /// <para>
    /// Identity uses a fixed cookie name: <c>cfd.session</c>.
    /// </para>
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the plaintext session token stored in the cookie.
    /// <para>
    /// This value is never persisted server-side; only the hash is stored.
    /// </para>
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new <see cref="SessionCookie"/> with the specified name and value.
    /// </summary>
    /// <param name="name">The cookie name.</param>
    /// <param name="value">The plaintext session token.</param>
    private SessionCookie(string name, string value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Creates a <see cref="SessionCookie"/> from a plaintext session token.
    /// <para>
    /// The cookie name is fixed to <c>cfd.session</c>, ensuring consistency
    /// across all authentication flows.
    /// </para>
    /// </summary>
    /// <param name="token">The plaintext session token.</param>
    /// <returns>A new <see cref="SessionCookie"/> instance.</returns>
    public static SessionCookie FromPlaintextToken(string token)
        => new("cfd.session", token);

    /// <summary>
    /// Returns the cookie in standard HTTP header format: <c>Name=Value</c>.
    /// </summary>
    public override string ToString() => $"{Name}={Value}";

    /// <summary>
    /// Defines the components used to determine equality between cookie instances.
    /// </summary>
    /// <returns>
    /// The sequence of components that uniquely identify this value object.
    /// </returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Value;
    }
}
