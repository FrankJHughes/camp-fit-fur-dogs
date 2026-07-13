using Frank.Core.Application.Abstractions.Audit;
using Microsoft.Extensions.Logging;

namespace Frank.Core.Infrastructure.Audit;

public class AuditLogger : IAuditLogger
{
    private readonly ILogger<AuditLogger> _logger;

    public AuditLogger(ILogger<AuditLogger> logger)
    {
        _logger = logger;
    }

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
