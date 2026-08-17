#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Frank.Core.Api.Platform.Logging;

/// <summary>
/// Provides extension methods for enabling platform-level logging features
/// for the Frank.Core API.
/// <para>
/// This subsystem activates HTTP request/response logging when the application
/// is running in the Development environment.
/// It ensures that diagnostic visibility is improved during local development
/// without impacting production performance or security.
/// </para>
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Enables platform logging features for the Frank.Core API.
    /// <para>
    /// When running in the Development environment, this method:
    /// <list type="bullet">
    /// <item><description>Enables ASP.NET Core HTTP logging via <c>UseHttpLogging()</c>.</description></item>
    /// <item><description>Emits a startup log entry confirming that HTTP logging is active.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// In non‑development environments, no logging features are added, ensuring
    /// production remains optimized and free of verbose diagnostics.
    /// </para>
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
    /// <returns>
    /// The same <see cref="WebApplication"/> instance, enabling fluent configuration.
    /// </returns>
    public static WebApplication UseFrankCoreApiPlatformLogging(this WebApplication app)
    {
        var env = app.Services.GetRequiredService<IWebHostEnvironment>();
        var logger = app.Services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Startup.Logging");

        if (env.IsDevelopment())
        {
            app.UseHttpLogging();
            logger.LogInformation("HTTP logging enabled for Development environment.");
        }

        return app;
    }
}
