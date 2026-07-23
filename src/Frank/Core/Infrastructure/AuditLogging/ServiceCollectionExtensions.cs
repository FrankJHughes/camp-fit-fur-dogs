using Frank.Core.Application.Abstractions.Audit;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Infrastructure.AuditLogging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreInfrastructureAuditLogging(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAuditLogger, AuditLogger>()
            ;
    }
}
