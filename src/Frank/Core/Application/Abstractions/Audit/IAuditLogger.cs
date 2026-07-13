namespace Frank.Core.Application.Abstractions.Audit;

public interface IAuditLogger
{
    Task LoginSucceeded(Guid userId, string externalId);
}
