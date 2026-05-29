using System;
using System.Threading.Tasks;
using CampFitFurDogs.Application.Abstractions.Audit;
using Microsoft.Extensions.Logging;

namespace CampFitFurDogs.Infrastructure.Audit;

public class AuditLogger : IAuditLogger
{
    private readonly ILogger<AuditLogger> _logger;

    public AuditLogger(ILogger<AuditLogger> logger)
    {
        _logger = logger;
    }

    public Task LoginSucceeded(Guid customerId, string externalId)
    {
        // Structured logging for audit trails
        _logger.LogInformation(
            "Audit: Owner login succeeded. CustomerId={CustomerId}, ExternalId={ExternalId}",
            customerId,
            externalId);

        return Task.CompletedTask;
    }
}
