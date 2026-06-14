using Microsoft.Extensions.Configuration;

namespace Frank.Api.Hosting;

public sealed class HostingOverridesConfigurationSource(IDictionary<string, string?> values) : IConfigurationSource
{
    private readonly IDictionary<string, string?> _values = values;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new HostingConfigurationProvider(_values);
}
