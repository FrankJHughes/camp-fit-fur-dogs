using Frank.Core.Infrastructure.AuditLogging;
using Frank.Core.Infrastructure.EnvironmentVariables;
using Frank.Core.Infrastructure.Exceptions;
using Frank.Core.Infrastructure.Clock;
using Microsoft.Extensions.DependencyInjection;
using Frank.Core.Infrastructure.Observations;

namespace Frank.Core.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreInfrastructure(this IServiceCollection services)
    {
        return services
            .AddFrankCoreInfrastructureAuditLogging()
            .AddFrankCoreInfrastructureClock()
            .AddFrankCoreInfrastructureEnvironmentVariables()
            .AddFrankCoreInfrastructureExceptions()
            .AddFrankCoreInfrastructureObservations()
            ;
    }
}
