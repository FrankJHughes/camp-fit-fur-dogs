using Frank.Core.Domain;

namespace Frank.Identity.Domain.Sessions.Errors;

/// <summary>
/// Represents domain-level errors related to invalid
/// <see cref="SessionTokenHash"/> values.
/// <para>
/// This exception is thrown when a session token hash fails to meet the
/// invariants required by the Identity domain. These invariants ensure that
/// every stored session token hash is non-empty and formatted as a valid
/// 64-character SHA-256 hexadecimal string.
/// </para>
/// <para>
/// All factory methods provide strongly-typed, intention-revealing error
/// creation for common invalid states.
/// </para>
/// </summary>
public sealed class InvalidSessionTokenHashException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="InvalidSessionTokenHashException"/> with the specified
    /// error message.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of the token hash violation.
    /// </param>
    public InvalidSessionTokenHashException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates an exception indicating that a token hash was empty or
    /// consisted only of whitespace.
    /// <para>
    /// Domain rule: A session token hash must always be a non-empty
    /// SHA-256 hexadecimal string.
    /// </para>
    /// </summary>
    /// <returns>
    /// A new <see cref="InvalidSessionTokenHashException"/> describing the
    /// empty-value violation.
    /// </returns>
    public static InvalidSessionTokenHashException Empty()
        => new("Token hash cannot be empty.");

    /// <summary>
    /// Creates an exception indicating that a raw string failed validation
    /// as a SHA-256 hex string.
    /// <para>
    /// Domain rule: Session token hashes must be valid 64-character
    /// lowercase hexadecimal strings representing SHA-256 output.
    /// </para>
    /// </summary>
    /// <param name="value">
    /// The raw string value that failed SHA-256 hex validation.
    /// </param>
    /// <returns>
    /// A new <see cref="InvalidSessionTokenHashException"/> describing the
    /// invalid-format violation.
    /// </returns>
    public static InvalidSessionTokenHashException InvalidFormat(string? value)
        => new($"Token hash '{value}' is not a valid 64-character SHA-256 hex string.");
}
