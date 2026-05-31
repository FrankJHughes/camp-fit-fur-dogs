using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers.Exceptions;

public sealed class InvalidCustomerIdException : DomainException
{
    public InvalidCustomerIdException(string message) : base(message) { }
}
