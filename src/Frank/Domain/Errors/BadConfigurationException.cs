namespace Frank.Domain.Errors;

public sealed class BadConfigurationException : DomainException
{
    public BadConfigurationException(string message)
        : base(message)
    {
    }
}
