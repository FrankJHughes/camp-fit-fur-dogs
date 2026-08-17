namespace Frank.Core.Domain;

/// <summary>
/// Represents the base class for all domain-specific exceptions.
///
/// Domain exceptions indicate violations of domain invariants or business rules.
/// They are part of the ubiquitous language and should be thrown only when
/// domain logic is broken or an operation cannot be completed due to
/// domain constraints.
///
/// This class inherits from <see cref="Exception"/> but should not be used
/// for infrastructure or application-level failures. Those belong in their
/// respective layers.
///
/// Example:
/// <code>
/// public sealed class InvalidDogNameException : DomainException
/// {
///     public InvalidDogNameException(string message) : base(message) { }
/// }
/// </code>
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class
    /// with the specified error message.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of the domain rule or invariant that was violated.
    /// </param>
    protected DomainException(string message)
        : base(message)
    {
    }
}
