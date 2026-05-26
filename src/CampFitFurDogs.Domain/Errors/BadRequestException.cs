using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Errors;

public class BadRequestException : DomainException
{
    public BadRequestException(string message)
        : base(message)
    {
    }
}
