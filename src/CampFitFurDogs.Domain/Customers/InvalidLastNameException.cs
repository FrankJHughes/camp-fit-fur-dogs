using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class InvalidLastNameException : DomainException
{
    public InvalidLastNameException(string message) : base(message) { }
}
