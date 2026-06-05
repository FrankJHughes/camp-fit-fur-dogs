using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers.Exceptions;

public sealed class InvalidPasswordHashException : DomainException
{
    public InvalidPasswordHashException(string message) : base(message) { }
}
