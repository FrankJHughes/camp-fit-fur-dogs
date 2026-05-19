using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class EmailAlreadyExistsException : DomainException
{
    public EmailAlreadyExistsException(string email)
        : base($"A customer with email '{email}' already exists.")
    {
    }
}
