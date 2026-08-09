namespace Frank.Core.Domain.Exceptions;

/// <summary>
/// Represents a domain-level exception indicating that a request
/// violated a domain rule, invariant, or precondition.
///
/// <para>
/// This exception should be thrown when the caller provides input
/// that is syntactically valid but semantically unacceptable within
/// the domain model.
/// </para>
///
/// <para>
/// Examples include:
/// <list type="bullet">
///   <item>Attempting to register a dog with an invalid name</item>
///   <item>Providing an email address that fails domain validation</item>
///   <item>Issuing a command that violates a business rule</item>
/// </list>
/// </para>
///
/// <para>
/// This exception is part of the ubiquitous language and should be
/// used only for domain-specific validation failures. Infrastructure
/// or application-level errors should use their respective exception
/// types.
/// </para>
/// </summary>
public class BadRequestException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class
    /// with the specified error message describing the domain rule violation.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of the domain rule or invariant that was violated.
    /// </param>
    public BadRequestException(string message)
        : base(message)
    {
    }
}
