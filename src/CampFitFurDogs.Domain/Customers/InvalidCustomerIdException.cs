using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class InvalidCustomerIdException : DomainException
{
    public InvalidCustomerIdException(string message) : base(message) { }
}
