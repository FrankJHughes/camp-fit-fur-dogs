using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers.Exceptions;

public sealed class InvalidLastNameException : DomainException
{
    public InvalidLastNameException(string message) : base(message) { }
}
