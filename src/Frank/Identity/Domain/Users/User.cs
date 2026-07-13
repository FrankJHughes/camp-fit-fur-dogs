using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    public FirstName FirstName { get; }
    public LastName LastName { get; }
    public Email Email { get; }
    public PhoneNumber? Phone { get; }
    public ExternalId ExternalId { get; }

#pragma warning disable CS8618
    private User() : base(default!)
    {
        // EF Core
    }
#pragma warning restore CS8618

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
    /// Creates a new User aggregate enforcing domain invariants:
    /// - External identity is required
    /// - Local identity is no longer supported
    /// </summary>
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
