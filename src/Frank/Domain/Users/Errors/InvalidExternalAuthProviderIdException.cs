namespace Frank.Domain.Users.Exceptions;

public sealed class InvalidExternalAuthProviderIdException : DomainException
{
    public InvalidExternalAuthProviderIdException(string message) : base(message) { }
}
