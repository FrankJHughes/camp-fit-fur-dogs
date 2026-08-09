using Frank.Identity.Application.Abstractions.AuditLogging;
using Microsoft.Extensions.Logging;

namespace Frank.Identity.Infrastructure.AuditLogging;

/// <summary>
/// Provides structured audit logging for authentication‑related events within
/// the Identity subsystem.
/// <para>
/// This logger emits machine‑readable, structured log entries that support
/// security monitoring, incident analysis, and compliance reporting.
/// It aligns with observability requirements from <see cref="US-183"/> and
/// authentication audit requirements from <see cref="US-110"/>.
/// </para>
/// <para>
/// The logger does not perform persistence or correlation; it delegates those
/// responsibilities to the hosting environment’s logging pipeline.
/// </para>
/// </summary>
public class AuditLogger : IAuditLogger
{
    private readonly ILogger<AuditLogger> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLogger"/> using the
    /// provided structured <see cref="ILogger{AuditLogger}"/>.
    /// </summary>
    /// <param name="logger">
    /// The logger used to emit structured audit events.
    /// </param>
    public AuditLogger(ILogger<AuditLogger> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Records a structured audit event indicating that an owner successfully
    /// authenticated using an external identity provider.
    /// <para>
    /// This event is emitted after a successful OIDC login and is used for:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Security monitoring</description></item>
    /// <item><description>Authentication audit trails</description></item>
    /// <item><description>Operational observability</description></item>
    /// </list>
    /// </summary>
    /// <param name="userId">The internal Identity user ID.</param>
    /// <param name="externalId">The external identity provider’s subject identifier.</param>
    /// <returns>A completed task.</returns>
    public Task LoginSucceeded(Guid userId, string externalId)
    {
        // Structured logging for audit trails
        _logger.LogInformation(
            "Audit: Owner login succeeded. UserId={UserId}, ExternalId={ExternalId}",
            userId,
            externalId);

        return Task.CompletedTask;
    }
}
