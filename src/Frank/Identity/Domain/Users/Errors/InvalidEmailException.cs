using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

public sealed class InvalidEmailException : DomainException
{
    public InvalidEmailException(string message) : base(message) { }
}
