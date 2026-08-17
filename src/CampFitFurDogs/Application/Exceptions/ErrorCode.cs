using Frank.Core.Application.Abstractions.Exceptions;

namespace CampFitFurDogs.Application.Exceptions;

/// <summary>
/// Represents a strongly‑typed error code used throughout the application layer
/// to provide consistent, machine‑readable identifiers for error conditions.
/// <para>
/// Error codes are used when throwing or returning <see cref="IErrorCode"/>
/// based exceptions, enabling API clients and internal components to reliably
/// interpret and react to specific failure scenarios.
/// </para>
/// <para>
/// Each error code consists of a short, stable string identifier and an optional
/// human‑readable description. The identifier is intended for programmatic use,
/// while the description provides additional context when needed.
/// </para>
/// </summary>
public sealed class ErrorCode : IErrorCode
{
    /// <summary>
    /// Gets the stable, machine‑readable identifier for the error.
    /// <para>
    /// This value is intended for programmatic use (e.g., logging, telemetry,
    /// client‑side error handling) and should remain stable across releases.
    /// </para>
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets an optional human‑readable description of the error.
    /// <para>
    /// This value is not required and may be <c>null</c>. When provided, it
    /// supplements the <see cref="Code"/> with additional context suitable for
    /// diagnostics or user‑facing error messages.
    /// </para>
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Initializes a new <see cref="ErrorCode"/> instance.
    /// <para>
    /// This constructor is private to ensure that all error codes are defined
    /// as static readonly fields, guaranteeing consistency and preventing
    /// accidental duplication.
    /// </para>
    /// </summary>
    /// <param name="code">The stable identifier for the error.</param>
    /// <param name="description">An optional human‑readable description.</param>
    private ErrorCode(string code, string? description = null)
    {
        Code = code;
        Description = description;
    }

    /// <summary>
    /// Indicates that an external authentication provider returned an error or
    /// failed to complete the authentication workflow.
    /// </summary>
    public static readonly ErrorCode ExternalAuthProviderFailure =
        new("external_auth_provider_failure");

    /// <summary>
    /// Indicates that the caller attempted an operation requiring authentication
    /// but no authenticated user was present.
    /// </summary>
    public static readonly ErrorCode UserNotAuthenticated =
        new("user_not_authenticated");

    /// <summary>
    /// Indicates that the authenticated user's identity was malformed, missing,
    /// or inconsistent with expected application rules.
    /// </summary>
    public static readonly ErrorCode InvalidUserIdentity =
        new("invalid_user_identity");

    /// <summary>
    /// Indicates that the application encountered an invalid or missing
    /// configuration value.
    /// </summary>
    public static readonly ErrorCode BadConfiguration =
        new("bad_configuration");

    /// <summary>
    /// Indicates that the caller provided invalid input or made a malformed
    /// request that cannot be processed.
    /// </summary>
    public static readonly ErrorCode BadRequest =
        new("bad_request");

    /// <summary>
    /// Indicates that an email address already exists in the system and cannot
    /// be used to create a new account.
    /// </summary>
    public static readonly ErrorCode DuplicateEmail =
        new("duplicate_email");

    /// <summary>
    /// Indicates that structural or semantic validation failed for the incoming
    /// request or command.
    /// </summary>
    public static readonly ErrorCode ValidationFailed =
        new("validation_failed");

    /// <summary>
    /// Indicates that a domain‑level invariant or rule was violated during
    /// execution of a command or query.
    /// </summary>
    public static readonly ErrorCode DomainError =
        new("domain_error");

    /// <summary>
    /// Indicates that an unexpected or unhandled error occurred.
    /// </summary>
    public static readonly ErrorCode Unexpected =
        new("unexpected");
}
