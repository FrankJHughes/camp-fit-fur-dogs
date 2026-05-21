using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class Customer : AggregateRoot<CustomerId>
{
    public FirstName FirstName { get; }
    public LastName LastName { get; }
    public Email Email { get; }
    public PhoneNumber Phone { get; }
    public PasswordHash PasswordHash { get; }

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
        PhoneNumber phone,
        PasswordHash passwordHash) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        PasswordHash = passwordHash;
    }

    public static Customer Create(
        FirstName firstName,
        LastName lastName,
        Email email,
        PhoneNumber phone,
        PasswordHash passwordHash)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(phone);
        ArgumentNullException.ThrowIfNull(passwordHash);

        return new Customer(
            CustomerId.New(),
            firstName,
            lastName,
            email,
            phone,
            passwordHash);
    }
}
