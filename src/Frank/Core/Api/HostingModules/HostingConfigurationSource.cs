using Microsoft.Extensions.Configuration;

namespace Frank.Core.Api.HostingModules;

/// <summary>
/// Represents a configuration source that injects a predefined set of
/// key/value pairs into the ASP.NET Core configuration pipeline.
/// <para>
/// This source is typically used during hosting initialization to supply
/// configuration overrides originating from external systems, platform modules,
/// or runtime‑generated hosting metadata.
/// </para>
/// <para>
/// When added to an <see cref="IConfigurationBuilder"/>, this source produces a
/// <see cref="HostingConfigurationProvider"/> that loads the supplied values
/// directly into the configuration system.
/// </para>
/// </summary>
public sealed class HostingOverridesConfigurationSource : IConfigurationSource
{
    private readonly IDictionary<string, string?> _values;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostingOverridesConfigurationSource"/>
    /// class using the provided dictionary of configuration values.
    /// </summary>
    /// <param name="values">
    /// A dictionary containing configuration keys and their corresponding values.
    /// These values will be injected into the configuration pipeline when the
    /// source is built.
    /// </param>
    public HostingOverridesConfigurationSource(IDictionary<string, string?> values)
    {
        _values = values;
    }

    /// <summary>
    /// Builds a <see cref="HostingConfigurationProvider"/> using the stored
    /// configuration values.
    /// <para>
    /// The returned provider is responsible for loading the values into the
    /// configuration system when the builder constructs the final
    /// <see cref="IConfiguration"/> instance.
    /// </para>
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IConfigurationBuilder"/> requesting the provider.
    /// </param>
    /// <returns>
    /// A new <see cref="HostingConfigurationProvider"/> containing the supplied
    /// configuration overrides.
    /// </returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new HostingConfigurationProvider(_values);
}
