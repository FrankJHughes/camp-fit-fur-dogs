using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class Customer : AggregateRoot<CustomerId>
{
    public FirstName FirstName { get; }
    public LastName LastName { get; }
    public Email Email { get; }
    public PhoneNumber? Phone { get; }
    public ExternalId ExternalId { get; }

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
        ExternalId externalId) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        ExternalId = externalId;
    }

    /// <summary>
    /// Creates a new Customer aggregate enforcing domain invariants:
    /// - External identity is required
    /// - Local identity is no longer supported
    /// </summary>
    public static Customer Create(
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

        return new Customer(
            id: CustomerId.New(),
            firstName: firstName,
            lastName: lastName,
            email: email,
            phone: phone,
            externalId: externalId);
    }
}
