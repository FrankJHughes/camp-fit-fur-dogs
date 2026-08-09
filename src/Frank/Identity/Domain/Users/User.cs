using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users;

/// <summary>
/// Represents an authenticated owner within the Identity domain.
/// <para>
/// The <see cref="User"/> aggregate encapsulates all identity‑related
/// information required for authentication, personalization, and
/// communication. It is intentionally minimal and strictly composed of
/// validated value objects.
/// </para>
/// <para>
/// Domain invariants enforced:
/// </para>
/// <list type="bullet">
/// <item><description>
/// A <see cref="ExternalId"/> is always required. Local identity has been
/// fully de‑featured (US‑184).
/// </description></item>
/// <item><description>
/// All personal information (first name, last name, email, phone) must be
/// validated through their respective value objects.
/// </description></item>
/// <item><description>
/// The aggregate root identifier (<see cref="UserId"/>) is always generated
/// internally via <see cref="UserId.New"/>.
/// </description></item>
/// </list>
/// </summary>
public sealed class User : AggregateRoot<UserId>
{
    /// <summary>
    /// Gets the owner's validated first name.
    /// </summary>
    public FirstName FirstName { get; }

    /// <summary>
    /// Gets the owner's validated last name.
    /// </summary>
    public LastName LastName { get; }

    /// <summary>
    /// Gets the owner's validated email address.
    /// </summary>
    public Email Email { get; }

    /// <summary>
    /// Gets the owner's optional phone number, normalized to E.164 format.
    /// </summary>
    public PhoneNumber? Phone { get; }

    /// <summary>
    /// Gets the external identity provider identifier (e.g., <c>auth0|abc123</c>).
    /// This is the authoritative identity source for the user.
    /// </summary>
    public ExternalId ExternalId { get; }

#pragma warning disable CS8618
    /// <summary>
    /// Parameterless constructor required by EF Core materialization.
    /// </summary>
    private User() : base(default!)
    {
        // EF Core
    }
#pragma warning restore CS8618

    /// <summary>
    /// Initializes a new <see cref="User"/> aggregate with all required
    /// identity components already validated.
    /// </summary>
    private User(
        UserId id,
        FirstName firstName,
        LastName lastName,
        Email email,
        PhoneNumber? phone,
        ExternalId externalId) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        ExternalId = externalId;
    }

    /// <summary>
    /// Creates a new <see cref="User"/> aggregate enforcing all domain
    /// invariants:
    /// <list type="bullet">
    /// <item><description>
    /// External identity is required and must be validated via <see cref="ExternalId"/>.
    /// </description></item>
    /// <item><description>
    /// Local identity is no longer supported (US‑184).
    /// </description></item>
    /// <item><description>
    /// All personal information must be provided as validated value objects.
    /// </description></item>
    /// </list>
    /// </summary>
    /// <param name="firstName">Validated first name.</param>
    /// <param name="lastName">Validated last name.</param>
    /// <param name="email">Validated email address.</param>
    /// <param name="externalId">Validated external identity provider ID.</param>
    /// <param name="phone">Optional validated phone number.</param>
    /// <returns>A fully constructed <see cref="User"/> aggregate.</returns>
    public static User Create(
        FirstName firstName,
        LastName lastName,
        Email email,
        ExternalId externalId,
        PhoneNumber? phone = null)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(externalId);

        return new User(
            id: UserId.New(),
            firstName: firstName,
            lastName: lastName,
            email: email,
            phone: phone,
            externalId: externalId);
    }
}
