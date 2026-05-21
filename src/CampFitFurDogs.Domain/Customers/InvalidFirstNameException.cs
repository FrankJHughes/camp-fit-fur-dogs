using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class InvalidFirstNameException : DomainException
{
    public InvalidFirstNameException(string message) : base(message) { }
}
