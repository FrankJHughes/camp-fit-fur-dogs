namespace Frank.Abstractions.Errors;

public interface IErrorCode
{
    /// <summary>
    /// A stable, application-defined identifier for this error.
    /// </summary>
    string Code { get; }

    /// <summary>
    /// A human-readable description of the error.
    /// Optional but useful for non-HTTP transports.
    /// </summary>
    string? Description => null;
}
