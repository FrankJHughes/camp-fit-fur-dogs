using Frank.Domain;

namespace CampFitFurDogs.Domain.Sessions.Errors;

public sealed class SessionNotFoundException : DomainException
{
    public SessionNotFoundException()
        : base("Session was not found.")
    {
    }
}
