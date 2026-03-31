namespace CampFitFurDogs.Domain.Guardians;

using CampFitFurDogs.SharedKernel;

public sealed class GuardianId : ValueObject
{
    public Guid Value { get; }

    private GuardianId(Guid value) => Value = value;

    public static GuardianId New() => throw new NotImplementedException();

    public static GuardianId From(Guid value) => throw new NotImplementedException();

    protected override IEnumerable<object?> GetEqualityComponents()
        => throw new NotImplementedException();
}
