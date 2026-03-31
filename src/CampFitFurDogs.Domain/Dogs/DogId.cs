namespace CampFitFurDogs.Domain.Dogs;

using CampFitFurDogs.SharedKernel;

public sealed class DogId : ValueObject
{
    public Guid Value { get; }

    private DogId(Guid value) => Value = value;

    public static DogId New() => throw new NotImplementedException();

    public static DogId From(Guid value) => throw new NotImplementedException();

    protected override IEnumerable<object?> GetEqualityComponents()
        => throw new NotImplementedException();
}
