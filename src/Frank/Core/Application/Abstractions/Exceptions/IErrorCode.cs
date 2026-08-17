namespace Frank.Core.Application.Abstractions.Exceptions;

/// <summary>
/// Represents a stable, application‑defined error identifier and optional
/// human‑readable description.
///
/// <para>
/// Implementations of <see cref="IErrorCode"/> provide a consistent way to
/// classify and describe errors across the application. The <see cref="Code"/>
/// property supplies a durable identifier suitable for logging, telemetry,
/// serialization, and client‑side handling. The optional
/// <see cref="Description"/> property offers additional context, especially
/// useful for non‑HTTP transports or debugging scenarios.
/// </para>
/// </summary>
public interface IErrorCode
{
    /// <summary>
    /// Gets a stable, application‑defined identifier for this error.
    ///
    /// <para>
    /// Error codes should be deterministic and should not change once defined,
    /// ensuring reliable correlation across logs, clients, and services.
    /// </para>
    /// </summary>
    string Code { get; }

    /// <summary>
    /// Gets an optional human‑readable description of the error.
    ///
    /// <para>
    /// This value is not required and defaults to <c>null</c>. It is useful
    /// when additional context is needed, particularly in non‑HTTP transports
    /// or diagnostic output.
    /// </para>
    /// </summary>
    string? Description => null;
}
