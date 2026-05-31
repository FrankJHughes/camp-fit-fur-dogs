using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Authentication.Sessions.Errors;

public sealed class InvalidSessionTokenHashException : DomainException
{
    public InvalidSessionTokenHashException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates an exception for empty or whitespace token hashes.
    /// </summary>
    public static InvalidSessionTokenHashException Empty()
        => new("Token hash cannot be empty.");

    /// <summary>
    /// Creates an exception for invalid SHA-256 hex strings.
    /// </summary>
    public static InvalidSessionTokenHashException InvalidFormat(string? value)
        => new($"Token hash '{value}' is not a valid 64-character SHA-256 hex string.");
}
