using Microsoft.AspNetCore.Builder;

namespace SharedKernel.Api.Hosting;

/// <summary>
/// Defines a pluggable hosting environment configuration.
/// Implement this interface for each hosting provider (e.g., Render, Azure, AWS)
/// to encapsulate provider-specific startup overrides such as connection string
/// resolution, TLS configuration, and environment detection.
/// </summary>
public interface IHostingProvider
{
    /// <summary>
    /// Display name used in diagnostic logging (e.g., "Render", "Azure App Service").
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Returns <c>true</c> when the current runtime environment belongs to this
    /// provider.  Typically detected by probing for provider-specific environment
    /// variables.  Must be safe to call at any time and must not throw.
    /// </summary>
    bool IsActive();

    /// <summary>
    /// Applies all provider-specific overrides to the application builder.
    /// Called only when <see cref="IsActive"/> returns <c>true</c>.
    /// </summary>
    Task ConfigureAsync(WebApplicationBuilder builder);
}
