namespace CampFitFurDogs.Domain.Guardians;

using CampFitFurDogs.SharedKernel;

public sealed class Guardian : AggregateRoot<GuardianId>
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;

    private Guardian(GuardianId id) : base(id) { }

    public static Guardian Create(string firstName, string lastName, string email, string phone)
        => throw new NotImplementedException();
}
