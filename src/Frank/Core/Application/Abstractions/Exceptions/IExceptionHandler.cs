using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Abstractions.Exceptions;

/// <summary>
/// Defines the contract for handling exceptions within the application's
/// centralized exception‑handling pipeline.
///
/// <para>
/// Implementations of <see cref="IExceptionHandler"/> determine whether they can
/// process a given exception, map it to a stable <see cref="IErrorCode"/>, and
/// produce a corresponding <see cref="ProblemDetails"/> response suitable for
/// API output. This abstraction enables layered, modular, and deterministic
/// exception handling across the application.
/// </para>
///
/// <para>
/// The <see cref="RegistrationAttribute"/> ensures that all exception handlers
/// are automatically registered with a singleton lifetime, allowing the
/// exception‑handling engine to discover and evaluate them in a predictable
/// order.
/// </para>
/// </summary>
[Registration(ServiceLifetime.Singleton)]
public interface IExceptionHandler
{
    /// <summary>
    /// Determines whether this handler can process the specified exception.
    ///
    /// <para>
    /// Handlers typically inspect the exception type or its metadata to decide
    /// whether they are responsible for producing an error response. Multiple
    /// handlers may exist, but only those returning <c>true</c> will participate
    /// in handling.
    /// </para>
    /// </summary>
    /// <param name="exception">
    /// The exception to evaluate.
    /// </param>
    /// <returns>
    /// <c>true</c> if this handler can process the exception; otherwise,
    /// <c>false</c>.
    /// </returns>
    bool CanHandle(System.Exception exception);

    /// <summary>
    /// Maps the specified exception to a stable, application‑defined
    /// <see cref="IErrorCode"/>.
    ///
    /// <para>
    /// Error codes provide durable identifiers for logging, telemetry, and
    /// client‑side handling. Implementations should ensure that each exception
    /// type maps to a consistent and meaningful error code.
    /// </para>
    /// </summary>
    /// <param name="exception">
    /// The exception to map.
    /// </param>
    /// <returns>
    /// An <see cref="IErrorCode"/> representing the error classification.
    /// </returns>
    IErrorCode GetErrorCode(System.Exception exception);

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance describing the error
    /// represented by the specified exception.
    ///
    /// <para>
    /// Implementations should populate fields such as <c>title</c>,
    /// <c>detail</c>, <c>status</c>, and <c>type</c> based on the exception and
    /// its mapped <see cref="IErrorCode"/>. The resulting object is suitable for
    /// returning to API clients in a standardized format.
    /// </para>
    /// </summary>
    /// <param name="exception">
    /// The exception to convert into a <see cref="ProblemDetails"/> response.
    /// </param>
    /// <returns>
    /// A <see cref="ProblemDetails"/> instance describing the error.
    /// </returns>
    ProblemDetails CreateProblemDetails(System.Exception exception);
}
