using CampFitFurDogs.Domain.Customers.Exceptions;
using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class CustomerId : AggregateId
{
    private CustomerId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new InvalidCustomerIdException("CustomerId cannot be empty.");
    }

    /// <summary>
    /// Creates a new unique CustomerId.
    /// </summary>
    public static CustomerId New() => new(Guid.NewGuid());

    /// <summary>
    /// Wraps an existing Guid into a CustomerId, enforcing domain invariants.
    /// </summary>
    public static CustomerId From(Guid value) => new(value);
}
