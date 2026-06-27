namespace Frank.Infrastructure.Exceptions;

public sealed class ExceptionHandlingOptions
{
    /// <summary>
    /// Include exception details (message, stack trace) in the ProblemDetails output.
    /// Should be enabled only in Development.
    /// </summary>
    public bool IncludeExceptionDetails { get; set; } = false;

    /// <summary>
    /// Include the error code in the ProblemDetails output.
    /// </summary>
    public bool IncludeErrorCode { get; set; } = true;

    /// <summary>
    /// Log unhandled exceptions using the configured logger.
    /// </summary>
    public bool LogUnhandledExceptions { get; set; } = true;
}
