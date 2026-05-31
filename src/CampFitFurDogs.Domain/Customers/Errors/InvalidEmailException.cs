using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers.Exceptions;

public sealed class InvalidEmailException : DomainException
{
    public InvalidEmailException(string message) : base(message) { }
}
