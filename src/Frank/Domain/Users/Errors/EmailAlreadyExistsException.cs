namespace Frank.Domain.Users.Exceptions;

public sealed class EmailAlreadyExistsException : DomainException
{
    public EmailAlreadyExistsException(string email)
        : base($"A user with email '{email}' already exists.")
    {
    }
}
