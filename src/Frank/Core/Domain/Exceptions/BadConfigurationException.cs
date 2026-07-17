namespace Frank.Core.Domain.Exceptions;

public sealed class BadConfigurationException : DomainException
{
    public BadConfigurationException(string message)
        : base(message)
    {
    }
}
