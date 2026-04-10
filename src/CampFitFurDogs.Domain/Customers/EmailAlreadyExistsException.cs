namespace CampFitFurDogs.Domain.Customers;

public sealed class EmailAlreadyExistsException : Exception
{
    public EmailAlreadyExistsException(string email)
        : base($"A customer with email '{email}' already exists.")
    {
    }
}
