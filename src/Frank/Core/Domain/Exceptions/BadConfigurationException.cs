namespace Frank.Core.Domain.Exceptions;

/// <summary>
/// Represents a domain-level exception indicating that the application's
/// configuration violates a domain rule, invariant, or required assumption.
///
/// <para>
/// This exception should be thrown when the system is misconfigured in a way
/// that prevents correct domain behavior. Examples include:
/// </para>
/// <list type="bullet">
///   <item>Missing required domain configuration values</item>
///   <item>Invalid configuration that breaks domain invariants</item>
///   <item>Incorrect wiring of domain services or factories</item>
/// </list>
///
/// <para>
/// This exception is part of the ubiquitous language and should be used only
/// for domain-specific configuration failures. Infrastructure or application
/// configuration errors should use their respective exception types.
/// </para>
/// </summary>
public sealed class BadConfigurationException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadConfigurationException"/> class
    /// with the specified error message describing the configuration problem.
    /// </summary>
    /// <param name="message">
    /// A human-readable description of the configuration issue that violates
    /// domain expectations or prevents correct domain operation.
    /// </param>
    public BadConfigurationException(string message)
        : base(message)
    {
    }
}
