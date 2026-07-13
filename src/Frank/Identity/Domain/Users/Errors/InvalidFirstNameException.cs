using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

public sealed class InvalidFirstNameException : DomainException
{
    public InvalidFirstNameException(string message) : base(message) { }
}
