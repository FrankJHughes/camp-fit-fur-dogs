using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class InvalidPasswordHashException : DomainException
{
    public InvalidPasswordHashException(string message) : base(message) { }
}
