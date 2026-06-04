using Frank.Domain;

namespace CampFitFurDogs.Domain.Errors;

public class ExternalAuthProviderException : DomainException
{
    public ExternalAuthProviderException(string message)
        : base(message)
    {
    }
}
