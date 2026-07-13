using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

public class MissingIdentitySourceException : DomainException
{
    public MissingIdentitySourceException(string message) : base(message)
    {
    }
}
