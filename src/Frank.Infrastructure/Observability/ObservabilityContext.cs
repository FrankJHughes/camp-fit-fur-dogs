using Frank.Abstractions.Observability;

namespace Frank.Infrastructure.Observability;

public sealed class ObservabilityContext : IObservabilityContext
{
    public string CorrelationId { get; init; } = default!;
    public string Slice { get; init; } = default!;
    public string Module { get; init; } = default!;
    public string Environment { get; init; } = default!;
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public IReadOnlyDictionary<string, object?> Metadata { get; init; }
        = new Dictionary<string, object?>();
}
