using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

public sealed class InvalidUserIdException : DomainException
{
    public InvalidUserIdException(string message) : base(message) { }
}
