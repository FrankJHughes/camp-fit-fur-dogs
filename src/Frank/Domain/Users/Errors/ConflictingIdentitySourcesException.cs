namespace Frank.Domain.Users.Exceptions;

public class ConflictingIdentitySourcesException : DomainException
{
    public ConflictingIdentitySourcesException(string message) : base(message)
    {
    }
}
