using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

public class ConflictingIdentitySourcesException : DomainException
{
    public ConflictingIdentitySourcesException(string message) : base(message)
    {
    }
}
