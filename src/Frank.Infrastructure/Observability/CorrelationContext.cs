using Frank.Abstractions.Observability;

namespace Frank.Infrastructure.Observability;

public sealed class CorrelationContext : ICorrelationContext
{
    public string Create()
        => Guid.NewGuid().ToString("N");

    public string Propagate(string? incoming)
        => string.IsNullOrWhiteSpace(incoming)
            ? Create()
            : incoming;
}
