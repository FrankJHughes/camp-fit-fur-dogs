using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Application.Abstractions.Hosting;

/// <summary>
/// Defines the contract for a hosting module that participates in the
/// application's startup pipeline.
///
/// <para>
/// Hosting modules encapsulate environment‑specific or startup‑specific
/// behaviors such as configuration loading, service initialization, feature
/// toggles, or infrastructure bootstrapping. The hosting engine discovers these
/// modules (typically via <see cref="HostingModuleAttribute"/>) and evaluates
/// them in a deterministic order.
/// </para>
///
/// <para>
/// Each module can determine whether it should be active for the current
/// environment and can optionally provide configuration overrides that modify
/// or augment the application's configuration before the host is built.
/// </para>
/// </summary>
public interface IHostingModule
{
    /// <summary>
    /// Determines whether this hosting module should be active for the given
    /// <see cref="WebApplicationBuilder"/>.
    ///
    /// <para>
    /// Modules may inspect environment variables, configuration values,
    /// hosting environment names, or other contextual information to decide
    /// whether they should participate in the startup pipeline.
    /// </para>
    /// </summary>
    /// <param name="builder">
    /// The <see cref="WebApplicationBuilder"/> providing access to configuration,
    /// environment, and services.
    /// </param>
    /// <returns>
    /// <c>true</c> if the module should be active; otherwise, <c>false</c>.
    /// </returns>
    bool IsActive(WebApplicationBuilder builder);

    /// <summary>
    /// Retrieves configuration overrides provided by this hosting module.
    ///
    /// <para>
    /// Overrides allow modules to supply or modify configuration values before
    /// the application host is fully constructed. This is useful for injecting
    /// environment‑specific settings, enabling or disabling features, or
    /// applying module‑scoped configuration adjustments.
    /// </para>
    ///
    /// <para>
    /// The returned dictionary maps configuration keys to their overridden
    /// values. A <c>null</c> value indicates that the key should be removed or
    /// unset if supported by the hosting engine.
    /// </para>
    /// </summary>
    /// <param name="builder">
    /// The <see cref="WebApplicationBuilder"/> used to evaluate and produce
    /// configuration overrides.
    /// </param>
    /// <returns>
    /// A dictionary of configuration key/value overrides supplied by the module.
    /// </returns>
    Task<IDictionary<string, string?>> GetConfigurationOverridesAsync(WebApplicationBuilder builder);
}
