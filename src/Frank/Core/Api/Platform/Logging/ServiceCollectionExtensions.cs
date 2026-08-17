#nullable enable
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Platform.Logging;

/// <summary>
/// Provides extension methods for configuring platform-level logging services
/// for the Frank.Core API.
/// <para>
/// This subsystem registers ASP.NET Core's built-in HTTP logging infrastructure
/// and configures which request/response fields should be captured.
/// These logging services are later activated conditionally at runtime
/// (typically only in Development) by <see cref="ApplicationBuilderExtensions"/>.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers HTTP logging services for the Frank.Core API.
    /// <para>
    /// This method configures <see cref="HttpLoggingOptions"/> to capture a
    /// minimal but useful set of diagnostic fields:
    /// <list type="bullet">
    /// <item><description><c>RequestPath</c></description></item>
    /// <item><description><c>RequestMethod</c></description></item>
    /// <item><description><c>ResponseStatusCode</c></description></item>
    /// </list>
    /// These fields provide high‑value insight during development without
    /// introducing excessive verbosity or performance overhead.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    public static IServiceCollection AddFrankCoreApiPlatformLogging(this IServiceCollection services)
    {
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields =
                HttpLoggingFields.RequestPath |
                HttpLoggingFields.RequestMethod |
                HttpLoggingFields.ResponseStatusCode;
        });

        return services;
    }
}
