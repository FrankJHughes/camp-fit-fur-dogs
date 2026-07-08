namespace Frank.Domain.Users.Exceptions;

public sealed class InvalidPasswordHashException : DomainException
{
    public InvalidPasswordHashException(string message) : base(message) { }
}
