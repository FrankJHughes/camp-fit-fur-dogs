using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class CustomerId : AggregateId
{
    // public Guid Value { get; }

    private CustomerId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty");
    }

    public static CustomerId New() => new(Guid.NewGuid());
    public static CustomerId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty.", nameof(value));
        return new CustomerId(value);
    }

}
