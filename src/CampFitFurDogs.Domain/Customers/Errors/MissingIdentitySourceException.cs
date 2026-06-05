using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers.Exceptions;

public class MissingIdentitySourceException : DomainException
{
    public MissingIdentitySourceException(string message) : base(message)
    {
    }
}
