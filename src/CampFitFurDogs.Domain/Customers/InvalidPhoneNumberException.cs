using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class InvalidPhoneNumberException : DomainException
{
    public InvalidPhoneNumberException(string message) : base(message) { }
}
