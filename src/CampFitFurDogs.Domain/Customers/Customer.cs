using CampFitFurDogs.Domain.Customers.Exceptions;
using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class Customer : AggregateRoot<CustomerId>
{
    public FirstName FirstName { get; }
    public LastName LastName { get; }
    public Email Email { get; }
    public PhoneNumber? Phone { get; }
    public PasswordHash? PasswordHash { get; }
    public ExternalAuthProviderId? ExternalAuthProviderId { get; private set; }

#pragma warning disable CS8618
    private Customer() : base(default!)
    {
        // EF Core
    }
#pragma warning restore CS8618

    private Customer(
        CustomerId id,
        FirstName firstName,
        LastName lastName,
        Email email,
        PhoneNumber? phone,
        PasswordHash? passwordHash,
        ExternalAuthProviderId? externalAuthProviderId) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        PasswordHash = passwordHash;
        ExternalAuthProviderId = externalAuthProviderId;
    }

    /// <summary>
    /// Creates a new Customer aggregate enforcing domain invariants:
    /// - Exactly one identity source must be provided (local OR external)
    /// - Local identity requires a valid PasswordHash
    /// - External identity requires a valid ExternalAuthProviderId
    /// </summary>
    public static Customer Create(
        FirstName firstName,
        LastName lastName,
        Email email,
        PhoneNumber? phone = null,
        PasswordHash? passwordHash = null,
        ExternalAuthProviderId? externalId = null)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(email);

        var hasLocal = passwordHash is not null;
        var hasExternal = externalId is not null;

        if (hasLocal && hasExternal)
            throw new ConflictingIdentitySourcesException(
                "Customer cannot have both a password hash and an external provider identity.");

        if (!hasLocal && !hasExternal)
            throw new MissingIdentitySourceException(
                "Customer must have either a password hash or an external provider identity.");

        return new Customer(
            CustomerId.New(),
            firstName,
            lastName,
            email,
            phone,
            passwordHash,
            externalId);
    }
}
