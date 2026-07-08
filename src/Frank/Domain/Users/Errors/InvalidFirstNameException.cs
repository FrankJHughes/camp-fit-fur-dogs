namespace Frank.Domain.Users.Exceptions;

public sealed class InvalidFirstNameException : DomainException
{
    public InvalidFirstNameException(string message) : base(message) { }
}
