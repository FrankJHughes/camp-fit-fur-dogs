namespace Frank.Core.Infrastructure.Exceptions;

/// <summary>
/// Provides configuration options for the exception‑handling subsystem.
/// <para>
/// These options control how exception details are exposed in
/// <c>ProblemDetails</c> responses and how unhandled exceptions are logged.
/// They allow environments (Development, Staging, Production) to adjust the
/// level of diagnostic information returned to clients.
/// </para>
/// </summary>
public sealed class ExceptionHandlingOptions
{
    /// <summary>
    /// Indicates whether exception details (message, stack trace) should be
    /// included in the <c>ProblemDetails</c> output.
    /// <para>
    /// This should be enabled only in Development environments, as exposing
    /// internal exception details in Production may leak sensitive information.
    /// </para>
    /// </summary>
    public bool IncludeExceptionDetails { get; set; } = false;

    /// <summary>
    /// Indicates whether the error code associated with an exception should be
    /// included in the <c>ProblemDetails</c> output.
    /// <para>
    /// Error codes are typically safe to expose and help clients understand the
    /// type of failure encountered.
    /// </para>
    /// </summary>
    public bool IncludeErrorCode { get; set; } = true;

    /// <summary>
    /// Indicates whether unhandled exceptions should be logged using the
    /// configured logging provider.
    /// <para>
    /// This should generally remain enabled to ensure operational visibility
    /// into unexpected failures.
    /// </para>
    /// </summary>
    public bool LogUnhandledExceptions { get; set; } = true;
}
