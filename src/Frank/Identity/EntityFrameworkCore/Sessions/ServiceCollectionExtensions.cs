using Frank.Identity.Application.Abstractions.Sessions.CreateSession;
using Frank.Identity.Application.Abstractions.Sessions.GetSession;
using Frank.Identity.Application.Abstractions.Sessions.RevokeSession;
using Frank.Identity.EntityFrameworkCore.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

/// <summary>
/// Provides extension methods for registering all Entity Framework Core–based
/// session components within the Identity subsystem.
/// <para>
/// This extension configures <see cref="SessionSettings"/> from application
/// configuration, validates them, and registers the three vertical‑slice
/// components responsible for creating, reading, and revoking sessions.
/// </para>
/// <para>
/// The method is intended to be called from application startup
/// (e.g., <c>Program.cs</c>), ensuring consistent configuration across all
/// hosting environments.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers EF Core session services and binds <see cref="SessionSettings"/>
    /// from configuration under the key <c>Identity:Session</c>.
    /// <para>
    /// The following services are registered:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="ICreateSessionWriter"/> → <see cref="CreateSessionWriter"/></description></item>
    /// <item><description><see cref="IGetSessionReader"/> → <see cref="GetSessionReader"/></description></item>
    /// <item><description><see cref="IRevokeSessionWriter"/> → <see cref="RevokeSessionWriter"/></description></item>
    /// </list>
    /// <para>
    /// <see cref="SessionSettings"/> is validated using data annotations and
    /// <c>ValidateOnStart</c> to ensure misconfiguration fails fast.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to which the session services will be added.</param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent
    /// registration chaining.
    /// </returns>
    public static IServiceCollection AddFrankIdentityEntityFrameworkCoreSessions(this IServiceCollection services)
    {
        services
            .AddOptions<SessionSettings>()
            .BindConfiguration("Identity:Session")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services
            .AddScoped<ICreateSessionWriter, CreateSessionWriter>()
            .AddScoped<IGetSessionReader, GetSessionReader>()
            .AddScoped<IRevokeSessionWriter, RevokeSessionWriter>();
    }
}
