using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

public sealed class InvalidPasswordHashException : DomainException
{
    public InvalidPasswordHashException(string message) : base(message) { }
}
