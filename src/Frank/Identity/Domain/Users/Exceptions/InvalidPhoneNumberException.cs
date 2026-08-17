using Frank.Core.Domain;

namespace Frank.Identity.Domain.Users.Exceptions;

/// <summary>
/// Represents a domain-level error indicating that a user's phone number
/// failed validation according to the rules of the Identity domain.
/// <para>
/// This exception is thrown when a phone number is empty, whitespace,
/// incorrectly formatted, fails normalization, or otherwise violates the
/// domain’s required structure for phone numbers.
/// </para>
/// <para>
/// Typical scenarios include:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Attempting to create or update a user with an invalid phone number.
/// </description></item>
/// <item><description>
/// Receiving malformed or incomplete phone-number data from an external
/// identity provider.
/// </description></item>
/// <item><description>
/// Violating domain rules requiring a well-formed, normalized, and
/// non-empty phone number.
/// </description></item>
/// </list>
/// </summary>
public sealed class InvalidPhoneNumberException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPhoneNumberException"/>
    /// with a message describing the validation failure.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of why the phone number is considered invalid.
    /// </param>
    public InvalidPhoneNumberException(string message) : base(message) { }
}
