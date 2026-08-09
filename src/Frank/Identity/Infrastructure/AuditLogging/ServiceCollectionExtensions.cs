using Frank.Identity.Application.Abstractions.AuditLogging;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Infrastructure.AuditLogging;

/// <summary>
/// Provides extension methods for registering audit‑logging infrastructure
/// components used by the Identity subsystem.
/// <para>
/// This extension registers <see cref="IAuditLogger"/> with a <c>Singleton</c>
/// lifetime, ensuring that audit events are emitted consistently across the
/// application and that the underlying logging pipeline manages concurrency,
/// batching, and sinks.
/// </para>
/// <para>
/// Audit logging supports authentication observability, security monitoring,
/// and compliance requirements by emitting structured, machine‑readable events.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Identity audit‑logging infrastructure.
    /// <para>
    /// Adds:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="IAuditLogger"/> → <see cref="AuditLogger"/></description></item>
    /// </list>
    /// <para>
    /// The audit logger is registered as a <c>Singleton</c> because it does not
    /// depend on scoped resources (such as DbContexts) and because audit events
    /// should be emitted through a single, consistent logging pipeline instance.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to which audit‑logging services will be added.</param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent
    /// registration chaining.
    /// </returns>
    public static IServiceCollection AddFrankIdentityInfrastructureAuditLogging(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAuditLogger, AuditLogger>();
    }
}
