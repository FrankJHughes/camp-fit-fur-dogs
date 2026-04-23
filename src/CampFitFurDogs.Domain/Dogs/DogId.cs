using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Dogs;

public sealed class DogId : AggregateId
{
    private DogId(Guid value) : base(value) { }

    public static DogId New() => new(Guid.NewGuid());

    public static DogId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("DogId cannot be empty.", nameof(value));
        return new DogId(value);
    }
}
