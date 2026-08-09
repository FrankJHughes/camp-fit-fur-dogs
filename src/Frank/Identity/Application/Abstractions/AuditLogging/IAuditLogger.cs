namespace Frank.Identity.Application.Abstractions.AuditLogging;

/// <summary>
/// Defines the contract for emitting audit events within the application.
///
/// <para>
/// Implementations of <see cref="IAuditLogger"/> are responsible for recording
/// significant system actions in a durable audit trail. These logs support
/// operational monitoring, compliance requirements, and forensic analysis.
/// </para>
/// </summary>
public interface IAuditLogger
{
    /// <summary>
    /// Records an audit event indicating that a user successfully completed
    /// a login operation within the system.
    ///
    /// <para>
    /// This event captures both the internal user identifier and the external
    /// identifier associated with the login context (for example, an ID issued
    /// by an upstream system or provider). The meaning of these identifiers is
    /// intentionally left to higher layers; this abstraction only records them.
    /// </para>
    /// </summary>
    /// <param name="userId">
    /// The internal identifier representing the user within the application.
    /// </param>
    /// <param name="externalId">
    /// An external identifier associated with the login event. The source and
    /// semantics of this value are defined by the caller, not by this interface.
    /// </param>
    Task LoginSucceeded(Guid userId, string externalId);
}
