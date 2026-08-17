using Frank.Core.Domain;

namespace Frank.Identity.Domain.Sessions.Errors;

/// <summary>
/// Represents domain-level errors related to invalid <see cref="SessionId"/> values.
/// <para>
/// This exception is thrown when a session identifier fails to meet the
/// invariants required by the Identity domain. These invariants ensure that
/// every session is uniquely identifiable, non-empty, and correctly formatted.
/// </para>
/// <para>
/// All factory methods provide strongly-typed, intention-revealing error
/// creation for common invalid states.
/// </para>
/// </summary>
public sealed class InvalidSessionIdException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidSessionIdException"/>
    /// with the specified error message.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of the session identifier violation.
    /// </param>
    public InvalidSessionIdException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates an exception indicating that a <see cref="SessionId"/> was
    /// constructed from <see cref="Guid.Empty"/>, which is not permitted.
    /// <para>
    /// Domain rule: A session identifier must always be a non-empty GUID.
    /// </para>
    /// </summary>
    /// <returns>
    /// A new <see cref="InvalidSessionIdException"/> describing the empty GUID violation.
    /// </returns>
    public static InvalidSessionIdException Empty()
        => new("SessionId cannot be empty.");

    /// <summary>
    /// Creates an exception indicating that a raw string could not be parsed
    /// into a valid <see cref="Guid"/> for use as a <see cref="SessionId"/>.
    /// <para>
    /// Domain rule: Session identifiers must be valid, non-empty GUID strings.
    /// </para>
    /// </summary>
    /// <param name="raw">
    /// The raw string value that failed GUID parsing.
    /// </param>
    /// <returns>
    /// A new <see cref="InvalidSessionIdException"/> describing the invalid format violation.
    /// </returns>
    public static InvalidSessionIdException InvalidFormat(string? raw)
        => new($"'{raw}' is not a valid SessionId (must be a non-empty GUID).");
}
