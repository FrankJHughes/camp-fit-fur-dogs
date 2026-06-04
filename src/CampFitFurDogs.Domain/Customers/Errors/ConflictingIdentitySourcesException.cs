using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers.Exceptions;

public class ConflictingIdentitySourcesException : DomainException
{
    public ConflictingIdentitySourcesException(string message) : base(message)
    {
    }
}
