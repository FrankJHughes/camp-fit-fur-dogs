namespace Frank.Domain.Users.Exceptions;

public class MissingIdentitySourceException : DomainException
{
    public MissingIdentitySourceException(string message) : base(message)
    {
    }
}
