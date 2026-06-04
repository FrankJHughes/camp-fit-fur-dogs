using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers.Exceptions;

public sealed class InvalidExternalAuthProviderIdException : DomainException
{
    public InvalidExternalAuthProviderIdException(string message) : base(message) { }
}
