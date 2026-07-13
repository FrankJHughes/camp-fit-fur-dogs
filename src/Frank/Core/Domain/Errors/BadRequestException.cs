namespace Frank.Core.Domain.Errors;

public class BadRequestException : DomainException
{
    public BadRequestException(string message)
        : base(message)
    {
    }
}
