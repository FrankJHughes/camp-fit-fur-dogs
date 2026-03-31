namespace CampFitFurDogs.Domain.Guardians;

using CampFitFurDogs.SharedKernel;

public sealed class Guardian : AggregateRoot<GuardianId>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }

    private Guardian(GuardianId id, string firstName, string lastName, string email, string phone)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
    }

    public static Guardian Create(string firstName, string lastName, string email, string phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        return new Guardian(GuardianId.New(), firstName, lastName, email, phone);
    }
}
