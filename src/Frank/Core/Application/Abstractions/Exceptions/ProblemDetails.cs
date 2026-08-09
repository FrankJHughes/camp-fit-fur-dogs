namespace Frank.Core.Application.Abstractions.Exceptions;

/// <summary>
/// Represents a standardized error response structure used to convey
/// machine‑readable details about an error condition.
///
/// <para>
/// This model aligns with the general shape of RFC 7807 “Problem Details for
/// HTTP APIs,” providing a consistent format for communicating errors across
/// transports. It is used by exception handlers to produce structured,
/// client‑friendly error responses.
/// </para>
/// </summary>
public class ProblemDetails
{
    /// <summary>
    /// A short, human‑readable summary of the problem.
    ///
    /// <para>
    /// This value should be concise and suitable for display. It typically
    /// corresponds to the high‑level classification of the error.
    /// </para>
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// A detailed, human‑readable explanation of the error.
    ///
    /// <para>
    /// This field may include contextual information helpful for debugging or
    /// understanding the cause of the failure. It should not expose sensitive
    /// data.
    /// </para>
    /// </summary>
    public string Detail { get; set; } = default!;

    /// <summary>
    /// The HTTP status code associated with the error, if applicable.
    ///
    /// <para>
    /// This value is optional to support non‑HTTP transports or scenarios where
    /// a status code is not meaningful.
    /// </para>
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// A URI‑identifying string that categorizes the error type.
    ///
    /// <para>
    /// This value may reference documentation or a canonical error category.
    /// It helps clients programmatically distinguish between different classes
    /// of errors.
    /// </para>
    /// </summary>
    public string Type { get; set; } = default!;

    /// <summary>
    /// A collection of validation or field‑specific errors.
    ///
    /// <para>
    /// Keys represent field names, and values contain one or more error
    /// messages associated with that field. This structure is useful for
    /// returning detailed validation feedback to clients.
    /// </para>
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }
}
