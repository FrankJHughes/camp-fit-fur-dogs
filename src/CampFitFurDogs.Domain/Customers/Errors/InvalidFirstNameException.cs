using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers.Exceptions;

public sealed class InvalidFirstNameException : DomainException
{
    public InvalidFirstNameException(string message) : base(message) { }
}
