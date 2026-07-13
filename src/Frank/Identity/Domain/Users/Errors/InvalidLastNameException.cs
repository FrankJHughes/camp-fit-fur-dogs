using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

public sealed class InvalidLastNameException : DomainException
{
    public InvalidLastNameException(string message) : base(message) { }
}
