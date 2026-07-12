namespace Frank.Domain.Sessions.Errors;

public sealed class SessionNotFoundException : DomainException
{
    public SessionNotFoundException()
        : base("Session was not found.")
    {
    }
}
