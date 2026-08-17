using Microsoft.Extensions.Configuration;

namespace Frank.Core.Api.HostingModules;

/// <summary>
/// Provides a lightweight configuration provider that injects a predefined set
/// of key/value pairs into the ASP.NET Core configuration system.
/// <para>
/// This provider is typically used during hosting or platform initialization to
/// supply configuration values originating from external systems, bootstrap
/// modules, or runtime‑generated hosting metadata.
/// </para>
/// <para>
/// Values added by this provider override earlier configuration sources but may
/// themselves be overridden by later ones, depending on the order in which
/// providers are added to the <see cref="IConfigurationBuilder"/>.
/// </para>
/// </summary>
public sealed class HostingConfigurationProvider : ConfigurationProvider
{
    private readonly IDictionary<string, string?> _values;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostingConfigurationProvider"/>
    /// class using the provided dictionary of configuration values.
    /// </summary>
    /// <param name="values">
    /// A dictionary containing configuration keys and their corresponding values.
    /// Keys should follow standard configuration naming conventions such as
    /// <c>"Logging:LogLevel:Default"</c> or <c>"ConnectionStrings:Default"</c>.
    /// </param>
    public HostingConfigurationProvider(IDictionary<string, string?> values)
    {
        _values = values;
    }

    /// <summary>
    /// Loads the configuration values into the provider's internal data store.
    /// <para>
    /// This method is invoked by the configuration system when building the
    /// final <see cref="IConfiguration"/> instance. Each key/value pair supplied
    /// to the provider is copied into the <see cref="ConfigurationProvider.Data"/>
    /// dictionary.
    /// </para>
    /// </summary>
    public override void Load()
    {
        foreach (var kvp in _values)
            Data[kvp.Key] = kvp.Value;
    }
}
