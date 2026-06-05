using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers.Exceptions;

public sealed class InvalidPhoneNumberException : DomainException
{
    public InvalidPhoneNumberException(string message) : base(message) { }
}
