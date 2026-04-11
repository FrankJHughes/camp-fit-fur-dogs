using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Domain.Customers;

public sealed class Customer : AggregateRoot<CustomerId>
{
    // public CustomerId Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public Email Email { get; }
    public PhoneNumber Phone { get; }
    public PasswordHash PasswordHash { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Customer() : base(default!)
    {
        // For EF Core
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Customer(
        CustomerId id,
        string firstName,
        string lastName,
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
        string firstName,
        string lastName,
        Email email,
        PhoneNumber phone,
        PasswordHash passwordHash)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

        return new Customer(
            CustomerId.New(),
            firstName,
            lastName,
            email,
            phone,
            passwordHash);
    }
}
