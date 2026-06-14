using Microsoft.Extensions.Configuration;

namespace Frank.Api.Hosting;

public sealed class HostingConfigurationProvider(IDictionary<string, string?> values) : ConfigurationProvider
{
    private readonly IDictionary<string, string?> _values = values;

    public override void Load()
    {
        foreach (var kvp in _values)
            Data[kvp.Key] = kvp.Value;
    }
}
