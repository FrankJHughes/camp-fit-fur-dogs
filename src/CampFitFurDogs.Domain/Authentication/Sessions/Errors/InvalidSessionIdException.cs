using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Authentication.Sessions.Errors;

public sealed class InvalidSessionIdException : DomainException
{
    public InvalidSessionIdException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Thrown when a SessionId is created from Guid.Empty.
    /// </summary>
    public static InvalidSessionIdException Empty()
        => new("SessionId cannot be empty.");

    /// <summary>
    /// Thrown when parsing a string that is not a valid Guid.
    /// </summary>
    public static InvalidSessionIdException InvalidFormat(string? raw)
        => new($"'{raw}' is not a valid SessionId (must be a non-empty GUID).");
}
