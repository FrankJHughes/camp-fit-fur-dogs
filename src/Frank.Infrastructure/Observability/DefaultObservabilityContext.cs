using Frank.Abstractions.Observability;
using Microsoft.Extensions.Hosting;

namespace Frank.Infrastructure.Observability;

public sealed class DefaultObservabilityContext : IObservabilityContext
{
    public string CorrelationId { get; } = "none";
    public string Channel { get; } = "none";
    public string Agent { get; } = "none";
    public string Environment { get; }
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
    public IReadOnlyDictionary<string, object?> Metadata { get; }
        = new Dictionary<string, object?>();

    public DefaultObservabilityContext(IHostEnvironment env)
    {
        Environment = env.EnvironmentName;
    }
}
