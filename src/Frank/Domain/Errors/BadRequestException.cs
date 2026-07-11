namespace Frank.Domain.Errors;

public class BadRequestException : DomainException
{
    public BadRequestException(string message)
        : base(message)
    {
    }
}
