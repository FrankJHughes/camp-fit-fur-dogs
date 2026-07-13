using Frank.Core.Domain;

namespace Frank.Identity.Domain.Sessions.Errors;

public sealed class SessionNotFoundException : DomainException
{
    public SessionNotFoundException()
        : base("Session was not found.")
    {
    }
}
