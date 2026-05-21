using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class CustomerId : AggregateId
{
    private CustomerId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new InvalidCustomerIdException("CustomerId cannot be empty.");
    }

    public static CustomerId New() => new(Guid.NewGuid());
    public static CustomerId From(Guid value) => new(value);
}

// public sealed class InvalidCustomerIdException : DomainException
// {
//     public InvalidCustomerIdException(string message) : base(message) { }
// }
