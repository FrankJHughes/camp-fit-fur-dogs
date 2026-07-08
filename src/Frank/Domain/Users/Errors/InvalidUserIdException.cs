namespace Frank.Domain.Users.Exceptions;

public sealed class InvalidUserIdException : DomainException
{
    public InvalidUserIdException(string message) : base(message) { }
}
